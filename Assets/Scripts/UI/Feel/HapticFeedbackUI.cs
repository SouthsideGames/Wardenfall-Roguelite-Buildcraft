
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
    }

    public void MediumVibrate() 
    {
        if(!canVibrate)
           return;

    }

    public void HighVibrate()
    {
        if (!canVibrate)
            return;


    }

    private void VibrateStateChangedCallback(bool _vibrateState) => canVibrate = _vibrateState;
}
