using UnityEngine;

public class VoltageSpikeEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private float radius = 5f;
    [SerializeField] private int maxTargets = 5;
    [SerializeField] private float paralyzeDuration = 1f;
    [SerializeField] private LayerMask enemyMask;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null)
        {
            Debug.LogWarning("VoltageSpikeEffect: Target is null.");
            return;
        }

        Vector2 origin = target.transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, enemyMask);

        int hitsApplied = 0;
        foreach (Collider2D hit in hits)
        {
            if (hitsApplied >= maxTargets) break;

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)card.effectValue, false);

                EnemyStatus status = enemy.GetComponent<EnemyStatus>();
                if (status != null)
                {
                    StatusEffect paralyzeEffect = new StatusEffect(StatusEffectType.Paralyze, paralyzeDuration);
                    status.ApplyEffect(paralyzeEffect);
                }

                hitsApplied++;
            }
        }

        Destroy(gameObject);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
} 
