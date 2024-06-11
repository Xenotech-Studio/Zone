using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTrack : MonoBehaviour
{
    public Color changingColor;
    public ParticleSystem ps;
    ParticleSystem.Particle[] particles;


    // Update is called once per frame
    void Update()
    {
        int maxParticles = ps.main.maxParticles;
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }
        var coloroverlifetime = ps.colorOverLifetime;
        Gradient grad = coloroverlifetime.color.gradient;
        GradientColorKey[] refColorKey = new GradientColorKey[2];
        refColorKey[0].color = changingColor;
        refColorKey[0].time = 0.0f;
        refColorKey[1].color = changingColor;
        grad.SetKeys(refColorKey, grad.alphaKeys);
        coloroverlifetime.color = grad;
        
    }
}
