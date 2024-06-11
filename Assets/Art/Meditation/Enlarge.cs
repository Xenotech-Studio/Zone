using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enlarge : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public Vector3 InitialScale;

    private void Start()
    {
        InitialScale = transform.localScale;
    }
    
    public void EnlargeTo(float scale)
    {
        transform.localScale = InitialScale * scale;
    }
    
    public void ResetToOriginal()
    {
        transform.localScale = InitialScale;
    }
}
