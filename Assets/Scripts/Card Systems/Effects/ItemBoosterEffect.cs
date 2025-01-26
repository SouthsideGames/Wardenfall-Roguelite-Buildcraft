using UnityEngine;

public class ItemBoosterEffect : ICardEffect
{
    private float activeTime;
    private float boostAmount;

    public ItemBoosterEffect(float _boostAmount)
    {
        boostAmount = _boostAmount;
    }

    public void Activate(float duration)
    {
        activeTime = duration;
        ItemManager.Instance.ApplyItemBoost(boostAmount, activeTime);
    }

    public void Disable()
    {
     
    }
}
