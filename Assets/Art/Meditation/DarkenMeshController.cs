using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DarkenMeshController : MonoBehaviour
{
    public float DarkenTimer = 0f;
    public float DarkenTime = 2f;
    public bool IsDarkening  = false;
    
    public float DarkenAlpha = 0.5f;
    
    public Material DarkenMaterial;
    public float LightenTimer = 0f;
    public float LightenTime = 2f;
    public bool IsLightening  = false;
    
    public float LightenAlpha = 0f;

    public UnityEvent OnLightenFinish;
    
    public void StartDarken()
    {
        IsDarkening = true;
        DarkenTimer = DarkenTime;
        Update();
    }
    
    public void StartLighten()
    {
        IsLightening = true;
        LightenTimer = LightenTime;
    }

    private void Update()
    {
        if (IsDarkening)
        {
            DarkenTimer -= Time.deltaTime;
            
            Debug.Log("DarkenTimer: " + DarkenTimer);

            // lerp alpha between LightenAlpha and DarkenAlpha
            DarkenMaterial.SetColor("_Color0", new Color(0, 0, 0, DarkenAlpha * (1 - DarkenTimer / DarkenTime) + LightenAlpha * (DarkenTimer / DarkenTime)));
            
            if (DarkenTimer < 0f)
            {
                IsDarkening = false;
                DarkenTimer = 0f;
            }
        }
         
        if (IsLightening)
        {
            LightenTimer -= Time.deltaTime;

            if (LightenTimer > 0f)
            {
                DarkenMaterial.SetColor("_Color0", new Color(0, 0, 0,  DarkenAlpha * (LightenTimer / LightenTime) + LightenAlpha * (1 - LightenTimer / LightenTime)));
            }
            else if (LightenTimer < 0f)
            {
                IsLightening = false;
                LightenTimer = 0f;
                OnLightenFinish?.Invoke();
            }
        }
    }

    private void OnEnable()
    {
        StartDarken();
    }
}
