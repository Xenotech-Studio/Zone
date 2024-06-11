using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlackInOut : MonoBehaviour
{
    public Image ImageComponent;
    
    public float FadeTime = 1f;
    
    private float _progress;

    public bool Inverse;

    public UnityEvent OnAnimationFinish;
    
    public float Delay = 0.5f;

    public Transform CamBlack;
    
    // Update is called once per frame
    void Update()
    {
        if (_progress < 1f + Delay/FadeTime  && !Inverse)
        {
            _progress += Time.deltaTime / FadeTime;
            ImageComponent.color = new Color(0, 0, 0, _progress);
        }
        else if (_progress > 0f && Inverse)
        {
            _progress -= Time.deltaTime / FadeTime;
            ImageComponent.color = new Color(0, 0, 0, _progress);
        }
        
        if (_progress >= (1f + Delay/FadeTime) && !Inverse)
        {
            OnAnimationFinish?.Invoke();
            if(CamBlack!=null) CamBlack.gameObject.SetActive(true);
        }
        else if (_progress <= 0f && Inverse)
        {
            OnAnimationFinish?.Invoke();
        }
    }

    private void OnEnable()
    {
        _progress = Inverse ? 1f + Delay/FadeTime : 0f;
        
        if(Inverse) Invoke(nameof(DisableBlack), 0.1f);
    }

    public void DisableBlack()
    {
        if(CamBlack!=null) CamBlack.gameObject.SetActive(false);
    }
}
