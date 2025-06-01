using System.Collections;
using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timerText;

    public AudioClip tickSound;
    
    public void UpdateWaveText(string waveString) => waveText.text = waveString;    
    public void UpdateTimerText(string timerString) => timerText.text = timerString;    
    public void StageCompleted()
    {
        UpdateWaveText("Stage Completed");
        UpdateTimerText("");
    }

    public void SetTimerColor(Color color)
    {
        timerText.color = color;
    }

    private bool isFlashing = false;
    private Coroutine flashRoutine;

    public void FlashTimerUI()
    {
        if (isFlashing) return;

        isFlashing = true;
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        while (true)
        {
            timerText.enabled = !timerText.enabled;
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void HideWaveText() => waveText.gameObject.SetActive(false); 
}
