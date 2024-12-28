using UnityEngine;

public class StatusEffect
{
    
    public StatusEffectType Type { get; private set; }
    public int Damage { get; private set; }
    public float Duration { get; private set; }
    public float Interval { get; private set; }
    private Enemy enemy;

    public StatusEffect(StatusEffectType type, int damage, float duration, float interval, Enemy target)
    {
        Type = type;
        Damage = damage;
        Duration = duration;
        Interval = interval;
        enemy = target;
    }

    public void ApplyDamage()
    {
        enemy.TakeDamage(Damage, false);
    }
}
