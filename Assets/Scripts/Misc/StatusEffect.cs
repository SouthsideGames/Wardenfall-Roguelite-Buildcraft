using System;
using UnityEngine;

public class StatusEffect
{

    public StatusEffectType EffectType;
    public float Duration;
    public float Value;
    public float Interval;

    public int StackCount { get; private set; } = 1;
    public int MaxStacks = 3;
    public StackBehavior StackBehavior = StackBehavior.Refresh;

    public Action OnResetTimer;

    public bool IsExpired => Duration <= 0;

    public StatusEffect(StatusEffectType type, float duration, float value = 0f, float interval = 1f, int maxStacks = 3, StackBehavior stackBehavior = StackBehavior.Refresh)
    {
        EffectType = type;
        Duration = duration;
        Value = value;
        Interval = interval;
        MaxStacks = maxStacks;
        StackBehavior = stackBehavior;
    }

    public void ApplyStatModifiers(float casterPower, float targetResistance)
    {
        Duration *= 1 + (casterPower * 0.05f);
        Duration *= Mathf.Clamp01(1 - (targetResistance * 0.05f));
    }


    public void ApplyStack()
    {
        if (StackCount >= MaxStacks)
        {
            ResetDuration();
            return;
        }

        StackCount++;
        float previousValue = Value;
        float previousDuration = Duration;

        switch (StackBehavior)
        {
            case StackBehavior.Refresh:
                ResetDuration();
                break;
            case StackBehavior.Extend:
                Duration += previousDuration;
                break;
            case StackBehavior.AddValue:
                Value += previousValue;
                Duration = Mathf.Max(Duration, previousDuration);
                break;
        }
    }

    public void ResetDuration()
    {
        if (OnResetTimer != null)
            OnResetTimer.Invoke();
    }


    public StatusEffect Clone()
    {
        return new StatusEffect(EffectType, Duration, Value, Interval, MaxStacks, StackBehavior)
        {
            StackCount = this.StackCount
        };
    }
}
