using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticFeedbackUI : MonoBehaviour
{
    private bool canVibrate;

    private void Awake() 
    {
        SettingManager.onVibrateStateChanged += VibrateStateChangedCallback;

        RangedWeapon.OnBulletFired += LightVibrate;
        CharacterHealth.OnCharacterDeath += HighVibrate;

    }

    private void OnDestroy() 
    {
        SettingManager.onVibrateStateChanged -= VibrateStateChangedCallback;

        RangedWeapon.OnBulletFired -= LightVibrate;
        CharacterHealth.OnCharacterDeath -= HighVibrate;
    }

    public void LightVibrate() 
    {
        if(!canVibrate)
            return;
        else
            CandyCoded.HapticFeedback.HapticFeedback.LightFeedback();
    }

    public void MediumVibrate() 
    {
        if(!canVibrate)
           return;

        CandyCoded.HapticFeedback.HapticFeedback.MediumFeedback();
    }

    public void HighVibrate()
    {
        if (!canVibrate)
            return;

        CandyCoded.HapticFeedback.HapticFeedback.HeavyFeedback();

    }

    private void VibrateStateChangedCallback(bool _vibrateState) => canVibrate = _vibrateState;
}
