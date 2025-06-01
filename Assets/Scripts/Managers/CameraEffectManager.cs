
using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CameraEffectsManager : MonoBehaviour
{
    public static CameraEffectsManager Instance;

    [Header("CRT Filter")]
    [SerializeField] private Volume crtVolume;


    [Header("Audio")]
    [SerializeField] private AudioClip channelSwitchSound;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Triggers a TV-style channel switch transition with CRT flicker, sound, and CAM label.
    /// </summary>
    /// <param name="onComplete">Callback to execute after transition</param>
    public void PlayChannelSwitchEffect(Action onComplete = null)
    {

        StartCoroutine(TemporaryCRTBoost());

        AudioManager.Instance.PlaySFX(channelSwitchSound);

        LeanTween.delayedCall(0.6f, () => onComplete?.Invoke());
    }

    /// <summary>
    /// Temporarily boosts the CRT volume weight to simulate a transition flicker.
    /// </summary>
    private IEnumerator TemporaryCRTBoost()
    {
        float originalWeight = crtVolume.weight;

        LeanTween.value(gameObject, 0f, 1f, 0.2f).setOnUpdate(val => crtVolume.weight = val);
        yield return new WaitForSeconds(0.4f);
        LeanTween.value(gameObject, crtVolume.weight, originalWeight, 0.3f).setOnUpdate(val => crtVolume.weight = val);
    }

  

    /// <summary>
    /// Manually fade in CRT effect.
    /// </summary>
    public void EnableCRT(float duration = 1f)
    {
        LeanTween.value(gameObject, crtVolume.weight, 1f, duration)
            .setOnUpdate(val => crtVolume.weight = val);
    }

    /// <summary>
    /// Manually fade out CRT effect.
    /// </summary>
    public void DisableCRT(float duration = 1f)
    {
        LeanTween.value(gameObject, crtVolume.weight, 0f, duration)
            .setOnUpdate(val => crtVolume.weight = val);
    }
}
