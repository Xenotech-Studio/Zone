using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class DarkenEnvironment : MonoBehaviour
{
    public Light _mainLight;
    public Light MainLight => _mainLight ??= FindObjectOfType(typeof(Light), includeInactive: true) as Light;

    public Light PointLight;

    public bool DarkenSkybox = true;

    public Material FloorMat;

    public Volume BloomPostprocessing;

    public float MaxBloom = 70f;

    // Start is called before the first frame update
    public void Darken()
    {
        if (MainLight) MainLight.intensity = 0f;
        RenderSettings.ambientLight = new Color(190f/256f, 190f/256f, 190f/256f, 0);
        //RenderSettings.skybox.SetColor("TintColor",new Color(99f/256f, 99f/256f, 99f/256f, 0));

        if (DarkenSkybox)
        {
            RenderSettings.skybox.SetFloat("_Exposure", 0.35f);
        }
        

        PointLight.gameObject.SetActive(true);
        PointLight.intensity = 1f;
        FloorMat.color = new Color(100f/256f, 100f/256f, 100f/256f, 0);
        FloorMat.SetFloat("_Smoothness", 0.4f);
        
        // set post processing volume bloom intensity to 70
        BloomPostprocessing.profile.TryGet<Bloom>(out Bloom bloom);
        bloom.intensity.value = MaxBloom;
    }
    
    public void Lighten()
    {
        if (MainLight) MainLight.intensity = 1f;
        RenderSettings.ambientLight = new Color(226f/256f, 227f/256f, 231f/256f, 0);
        RenderSettings.skybox.SetColor("_TintColor",new Color(171f/256f, 171f/256f, 171f/256f, 0));
        RenderSettings.skybox.SetFloat("_Exposure", 0.84f);
        PointLight.gameObject.SetActive(false);
        FloorMat.color = new Color(168f/256f, 168f/256f, 168f/256f, 0);
        FloorMat.SetFloat("_Smoothness", 0.294f);
        
        // set post processing volume bloom intensity to 0
        BloomPostprocessing.profile.TryGet<Bloom>(out Bloom bloom);
        bloom.intensity.value = 0f;
    }

    public float LightenTimer = 0f;
    public bool LightenTimerWorking = false;
    public float LightenTimerMax = 1f;
    public void LightenWithTimer()
    {
        Darken();
        LightenTimerWorking = true;
        LightenTimer = LightenTimerMax;
        Update();
    }
    
    public float DarkenTimer = 0f;
    public bool DarkenTimerWorking = false;
    public float DarkenTimerMax = 1f;
    public void DarkenWithTimer()
    {
        Lighten();
        DarkenTimerWorking = true;
        DarkenTimer = DarkenTimerMax;
        Update();
    }
    
    // Update is called once per frame
    void Update()
    {
        
            
        if (DarkenTimerWorking)
        {
            Debug.Log("DarkenTimerWorking");
            
            DarkenTimer -= Time.deltaTime;
            
            // lerp between 0 and 1
            if (MainLight) MainLight.intensity = 1f - DarkenTimer / DarkenTimerMax;
            
            // lerp between dark (190f/256f, 190f/256f, 190f/256f, 0) and light (226f/256f, 227f/256f, 231f/256f, 0)
            RenderSettings.ambientLight = new Color(190f/256f, 190f/256f, 190f/256f, 0) * (1f - DarkenTimer / DarkenTimerMax) + 
                                          new Color(226f/256f, 227f/256f, 231f/256f, 0) * (DarkenTimer / DarkenTimerMax);
            
            // lerp between dark (99f/256f, 99f/256f, 99f/256f, 0) and light (171f/256f, 171f/256f, 171f/256f, 0)
            RenderSettings.skybox.SetColor("_TintColor",new Color(99f/256f, 99f/256f, 99f/256f, 0) * (1f - DarkenTimer / DarkenTimerMax) + 
                                                         new Color(171f/256f, 171f/256f, 171f/256f, 0) * (DarkenTimer / DarkenTimerMax));
            
            // lerp between 0.35 and 0.84
            RenderSettings.skybox.SetFloat("_Exposure", 0.35f * (1f - DarkenTimer / DarkenTimerMax) + 0.84f * (DarkenTimer / DarkenTimerMax));
            
            // lerp between 0 and 1
            PointLight.gameObject.SetActive(true);
            PointLight.intensity = 1f - DarkenTimer / DarkenTimerMax;
            
            // lerp floor color between dark  (100f/256f, 100f/256f, 100f/256f, 0) and light (168f/256f, 168f/256f, 168f/256f, 0)
            FloorMat.color =  new Color(100f/256f, 100f/256f, 100f/256f, 0) * (1f - DarkenTimer / DarkenTimerMax) + 
                              new Color(168f/256f, 168f/256f, 168f/256f, 0) * (DarkenTimer / DarkenTimerMax);
            
            // lerp between 0.4 and 0.294
            FloorMat.SetFloat("_Smoothness", 0.4f * (1f - DarkenTimer / DarkenTimerMax) + 0.294f * (DarkenTimer / DarkenTimerMax));
            
            // lerp between 70 and 0
            BloomPostprocessing.profile.TryGet<Bloom>(out Bloom bloom);
            bloom.intensity.value = MaxBloom * (1f - DarkenTimer / DarkenTimerMax) + 0f * (DarkenTimer / DarkenTimerMax);

            
            if (DarkenTimer <= 0f)
            {
                DarkenTimerWorking = false;
                Darken();
            }
        }

        
        if (LightenTimerWorking)
        {
            Debug.Log("LightenTimerWorking");
            
            LightenTimer -= Time.deltaTime;
            
            // lerp between 0 and 1
            if (MainLight) MainLight.intensity = LightenTimer / LightenTimerMax;
            
            // lerp between dark (190f/256f, 190f/256f, 190f/256f, 0) and light (226f/256f, 227f/256f, 231f/256f, 0)
            RenderSettings.ambientLight = new Color(190f/256f, 190f/256f, 190f/256f, 0) * (LightenTimer / LightenTimerMax) + 
                                          new Color(226f/256f, 227f/256f, 231f/256f, 0) * (1f - LightenTimer / LightenTimerMax);
            
            // lerp between dark (99f/256f, 99f/256f, 99f/256f, 0) and light (171f/256f, 171f/256f, 171f/256f, 0)
            RenderSettings.skybox.SetColor("_TintColor",new Color(99f/256f, 99f/256f, 99f/256f, 0) * (LightenTimer / LightenTimerMax) + 
                                                         new Color(171f/256f, 171f/256f, 171f/256f, 0) * (1f - LightenTimer / LightenTimerMax));
            
            // lerp between 0.35 and 0.84
            RenderSettings.skybox.SetFloat("_Exposure", 0.35f * (LightenTimer / LightenTimerMax) + 0.84f * (1f - LightenTimer / LightenTimerMax));
            
            // lerp between 0 and 1
            PointLight.gameObject.SetActive(true);
            PointLight.intensity = LightenTimer / LightenTimerMax;
            
            // lerp floor color between dark  (100f/256f, 100f/256f, 100f/256f, 0) and light (168f/256f, 168f/256f, 168f/256f, 0)
            FloorMat.color =  new  Color(100f/256f, 100f/256f, 100f/256f, 0) * (LightenTimer / LightenTimerMax) + 
                               new Color(168f/256f, 168f/256f, 168f/256f, 0) * (1f - LightenTimer / LightenTimerMax);
            
            // lerp between 0.4 and 0.294
            FloorMat.SetFloat("_Smoothness", 0.4f * (LightenTimer / LightenTimerMax) + 0.294f * (1f - LightenTimer / LightenTimerMax));
            
            // lerp between 70 and 0
            BloomPostprocessing.profile.TryGet<Bloom>(out Bloom bloom);
            bloom.intensity.value = MaxBloom * (LightenTimer / LightenTimerMax) + 0f * (1f - LightenTimer / LightenTimerMax);
            
            if (LightenTimer <= 0f)
            {
                LightenTimerWorking = false;
                Lighten();
            }
        }
    }


    private void OnEnable()
    {
        //Darken();
    }

    private void OnDisable()
    {
        Lighten();
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(DarkenEnvironment))]
public class DarkenEnvironmentEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Darken"))
        {
            DarkenEnvironment darkenEnvironment = target as DarkenEnvironment;
            darkenEnvironment.Darken();
            
        }
        if (GUILayout.Button("Lighten"))
        {
            DarkenEnvironment darkenEnvironment = target as DarkenEnvironment;
            darkenEnvironment.Lighten();
        }
        
        
    }
}
#endif