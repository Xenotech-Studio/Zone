using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MeditationControl : MonoBehaviour
{
    public float TotalTime;

    public float Timer;
    
    public float TimeLeft => TotalTime - Timer;

    public GameObject StartFlag;
    
    public int Minutes => (int) (TimeLeft / 60f);
    public int Seconds => (int) (TimeLeft % 60f);
    
    public string TimeString => $"{Minutes:00}:{Seconds:00}";

    public TMP_Text Text;

    public Slider ProgressBar;

    private void Update()
    {
        if(Timer<TotalTime && (StartFlag==null || StartFlag.activeSelf)) Timer += Time.deltaTime;
        
        if(Text!=null) Text.text = TimeString;
        
        if(ProgressBar!=null) ProgressBar.value = Timer / TotalTime;
    }

    private void OnEnable()
    {
        Timer = 0;
    }
}
