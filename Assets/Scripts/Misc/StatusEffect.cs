using UnityEngine;

public class StatusEffect
{

    public StatusEffectType EffectType;
    public float Duration;
    public float Value;
    public float Interval;

    public StatusEffect(StatusEffectType type, float duration, float value = 0, float interval = 0)
    {
        EffectType = type;
        Duration = duration;
        Value = value;
        Interval = interval;
    }
}
