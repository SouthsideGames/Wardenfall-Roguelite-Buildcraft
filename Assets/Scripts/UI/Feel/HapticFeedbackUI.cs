using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticFeedbackUI : MonoBehaviour
{
    public bool canVibrate;

    private void Awake() 
    {
        RangedWeapon.OnBulletFired += LightVibrate;
        CharacterHealth.OnCharacterDeath += HighVibrate;

    }

    private void OnDestroy() 
    {
        RangedWeapon.OnBulletFired -= LightVibrate;
        CharacterHealth.OnCharacterDeath -= HighVibrate;
    }

    public void LightVibrate() 
    {
        if(!canVibrate)
           return;

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
}
