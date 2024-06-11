using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//[ExecuteInEditMode]
public class PortalsFX2_GrvityPoint : MonoBehaviour
{
    public Transform Target;
    public Transform _headTransform;
    public Transform LeftHand;
    public Transform RightHand;
    public MeditationControl MeditationController;
    public GameObject jellyfish;
    public GameObject tutorhead;
    public Animator animator;
    public float InnerForce = 40;
    public float OuterForce = 20;
    public float Period = 1.2f;
    public float offset = 0.5f;
    public float StopDistance = 0;
    public float TouchDistance = 0.1f;
    public float DestroyDistance = 0.1f; // 添加这一行：销毁距离
    public float lefttime;
    public float acceleratingspeed = 500f;
    public AnimationCurve curve;
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;

    ParticleSystem.MainModule mainModule;
    
    private Vector3 _lastLeftHandPosition;
    private Vector3 _lastRightHandPosition;
    private Vector3 initscale;
    private bool timelimited = false;
    private bool played = false;

    private Dictionary<int, Vector3> ParticalAdditionSpeed = new Dictionary<int, Vector3>();

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        initscale = jellyfish.transform.localScale;
        mainModule = ps.main;
        _lastLeftHandPosition = LeftHand.position;
        _lastRightHandPosition = RightHand.position;
    }

    private void FixedUpdate()
    {
        if (MeditationController == null || MeditationController.TimeLeft < lefttime) timelimited = true;
        var maxParticles = mainModule.maxParticles;
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }
        int particleCount = ps.GetParticles(particles);
        for (int i = 0; i < particleCount; i++)
        {
            var targetTransformedPosition = transform.InverseTransformPoint(Target.position);
            var distanceToParticle = targetTransformedPosition - particles[i].position;
            
            if (!ParticalAdditionSpeed.ContainsKey(i))
            {
                ParticalAdditionSpeed.Add(i, Vector3.zero);
            }

            //ParticalAdditionSpeed[i] *= 0.98f;

            var directionToTarget = distanceToParticle.normalized;
            if (timelimited)
            {
                particles[i].velocity += curve.Evaluate(1f - MeditationController.TimeLeft/lefttime) * acceleratingspeed * directionToTarget;
                if (i==170) Debug.Log($"Velocity of {i}: "+particles[i].velocity.magnitude);
            }
        }
        
        if (timelimited)
        {
            jellyfish.transform.localScale = initscale * (1.3f - 0.3f*(MeditationController.TimeLeft / lefttime));
            if (!played) {
                animator.SetBool("End", true);
                played = true;
            }
        }
        
        if (lefttime < 1)
        {
            MeditationController.gameObject.SetActive(false);
        }
        
        
        ps.SetParticles(particles, particleCount);
    }

    void LateUpdate()
    {
        bool stopgenerate = false;
        if (MeditationController.TimeLeft < lefttime)
        {
            ps.enableEmission = false;
            stopgenerate = true;
        }
        var maxParticles = mainModule.maxParticles;
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }
        int particleCount = ps.GetParticles(particles);

        var targetTransformedPosition = Vector3.zero;
        var headTransformedPosition = Vector3.zero;
        var leftHandTransformedPosition = Vector3.zero;
        var rightHandTransformedPosition = Vector3.zero;
        var lastLeftHandTransformedPosition = Vector3.zero;
        var lastRightHandTransformedPosition = Vector3.zero;
        var tutorheadTransformedPosition = Vector3.zero;
        if (mainModule.simulationSpace == ParticleSystemSimulationSpace.Local)  //partical system的坐标系是local
        {
            targetTransformedPosition = transform.InverseTransformPoint(Target.position);
            headTransformedPosition = transform.InverseTransformPoint(_headTransform.position);
            leftHandTransformedPosition = transform.InverseTransformPoint(LeftHand.position);
            rightHandTransformedPosition = transform.InverseTransformPoint(RightHand.position);
            lastLeftHandTransformedPosition = transform.InverseTransformPoint(_lastLeftHandPosition);
            lastRightHandTransformedPosition = transform.InverseTransformPoint(_lastRightHandPosition);
            tutorheadTransformedPosition = transform.InverseTransformPoint(tutorhead.transform.position);
        }

        if (mainModule.simulationSpace == ParticleSystemSimulationSpace.World)
        {
            targetTransformedPosition = Target.position;
            headTransformedPosition = _headTransform.position;
            leftHandTransformedPosition = LeftHand.position;
            rightHandTransformedPosition = RightHand.position;
            lastLeftHandTransformedPosition = _lastLeftHandPosition;
            lastRightHandTransformedPosition = _lastRightHandPosition;
            tutorheadTransformedPosition = tutorhead.transform.position;
        }


        

        for (int i = 0; i < particleCount; i++)
        {

            if (!ParticalAdditionSpeed.ContainsKey(i))
            {
                ParticalAdditionSpeed.Add(i, Vector3.zero);
            }

            var distanceToParticle = targetTransformedPosition - particles[i].position;
            var distanceToHead = headTransformedPosition - particles[i].position;
            var distanceToLeftHand = leftHandTransformedPosition - particles[i].position;
            var distanceToRightHand = rightHandTransformedPosition - particles[i].position;
            var distanceTotutorhead = tutorheadTransformedPosition - particles[i].position;
           
            // 添加这一段：如果粒子距离目标太近，则销毁它
            
            if (distanceToParticle.magnitude < DestroyDistance || distanceToHead.magnitude < DestroyDistance || distanceTotutorhead.magnitude < 2.0f * DestroyDistance)
            {
                particles[i].remainingLifetime = 0;
                continue;
            }


            if (StopDistance > 0.001f && distanceToParticle.magnitude < StopDistance)
            {
                particles[i].velocity = Vector3.zero;
            }
            else
            {
                var directionToTarget = distanceToParticle.normalized;
                Vector3 seekForce;
                float sinvalue = Mathf.Sin((Time.time + offset) /  Period);
                if (stopgenerate)
                {
                    continue;
                }
                
                if (sinvalue > 0)
                {
                    seekForce = sinvalue * InnerForce * directionToTarget;
                }
                else
                {
                    seekForce = sinvalue * OuterForce * directionToTarget;
                }

                particles[i].velocity = seekForce;

            }

            if (distanceToLeftHand.magnitude < TouchDistance){
                HandTouchParticle(i, (leftHandTransformedPosition - lastLeftHandTransformedPosition) / (1+distanceToLeftHand.magnitude));
            }

            if (distanceToRightHand.magnitude < TouchDistance)
            {
                HandTouchParticle(i, (rightHandTransformedPosition - lastRightHandTransformedPosition) / (1+distanceToRightHand.magnitude));
            }
            
            particles[i].velocity += ParticalAdditionSpeed[i];
            
            //保证手可以推着同向的粒子运动
            Vector3 lefthandspeed = (leftHandTransformedPosition - lastLeftHandTransformedPosition) / Time.deltaTime; 
            Vector3 righthandspeed = (rightHandTransformedPosition - lastRightHandTransformedPosition) / Time.deltaTime;

            if (lefthandspeed.magnitude > particles[i].velocity.magnitude &&
                Vector3.Dot(lefthandspeed.normalized, particles[i].velocity.normalized) > 0.5f &&
                distanceToLeftHand.magnitude < TouchDistance)
            {
                Vector3 deltaVector = lefthandspeed - Vector3.Dot(particles[i].velocity, lefthandspeed.normalized) * lefthandspeed.normalized;
                particles[i].velocity += deltaVector;
            }
            if (righthandspeed.magnitude > particles[i].velocity.magnitude &&
                Vector3.Dot(righthandspeed.normalized, particles[i].velocity.normalized) > 0.5f &&
                distanceToRightHand.magnitude < TouchDistance)
            {
                Vector3 deltaVector = righthandspeed - Vector3.Dot(particles[i].velocity, righthandspeed.normalized) * righthandspeed.normalized;
                particles[i].velocity += deltaVector;
            }

                
        }


        _lastLeftHandPosition = LeftHand.position;
        _lastRightHandPosition = RightHand.position;
        ps.SetParticles(particles, particleCount);
    }

    private void OnDisable()
    {
        ParticalAdditionSpeed.Clear();
        jellyfish.transform.localScale = new Vector3(1,1,1);
        ps.enableEmission = true;
        timelimited = false;
    }

    private void HandTouchParticle(int i, Vector3 movePosition)
    {
        if (i >= particles.Length) return;
        ParticalAdditionSpeed[i] += 12 * movePosition/Time.deltaTime;
    }
}
