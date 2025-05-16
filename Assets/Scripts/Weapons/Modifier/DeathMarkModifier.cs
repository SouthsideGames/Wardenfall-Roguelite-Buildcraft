using UnityEngine;

public class DeathMarkModifier : MonoBehaviour, IBulletModifier
{
    [SerializeField] private float delay = 2f;
    [SerializeField] private float duration = 5f;
    [SerializeField] private float multiplier = 1.5f;

    public void Apply(BulletBase bullet, Enemy target)
    {
        if (target == null) return;

        EnemyStatus status = target.GetComponent<EnemyStatus>();
        if (status != null)
        {
            var effect = new StatusEffect(StatusEffectType.DeathMark, duration, multiplier, 0f);
            status.ApplyEffect(effect, delay);
        }
    }
}
