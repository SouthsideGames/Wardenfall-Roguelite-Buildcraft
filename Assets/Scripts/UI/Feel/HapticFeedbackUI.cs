using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticFeedbackUI : MonoBehaviour
{
    public bool canVibrate;

    private void Awake() 
    {
        RangedWeapon.OnBulletFired += LightVibrate;
        CharacterHealth.OnCharacterDeath += MediumVibrate;
    }

    private void OnDestroy() 
    {
        RangedWeapon.OnBulletFired -= LightVibrate;
        CharacterHealth.OnCharacterDeath -= MediumVibrate;
    }

    private void LightVibrate() 
    {
        if(!canVibrate)
           return;

        CandyCoded.HapticFeedback.HapticFeedback.LightFeedback();
    }

    private void MediumVibrate() 
    {
        if(!canVibrate)
           return;

        CandyCoded.HapticFeedback.HapticFeedback.MediumFeedback();
    }
}
