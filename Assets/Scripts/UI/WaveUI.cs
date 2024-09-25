using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timerText;
    
    public void UpdateWaveText(string waveString) => waveText.text = waveString;    
    public void UpdateTimerText(string timerString) => timerText.text = timerString;    
    public void StageCompleted()
    {
        UpdateWaveText("Stage Completed");
        UpdateTimerText("");
    }
}
