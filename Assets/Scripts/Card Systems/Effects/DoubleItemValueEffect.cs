using System;
using System.Collections;
using UnityEngine;

public class DoubleItemValueEffect : ICardEffect
{
   public static Action OnDoubleValueActivated;
    public static Action OnDoubleValueDeactivated;
   private float duration;

    public DoubleItemValueEffect(float _duration)
    {
        duration = _duration;
    }

    public void Activate(float duration)
    {
        OnDoubleValueActivated?.Invoke();
    }

    public void Disable()
    {
        OnDoubleValueDeactivated?.Invoke();
    }
}
