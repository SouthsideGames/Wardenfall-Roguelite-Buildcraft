using System.Collections;
using UnityEngine;

public class StatusEffectCardEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private StatusEffectType effectType;
    [SerializeField] private bool isPassive = true;
    [SerializeField] private float radius = 4f;
    [SerializeField] private LayerMask enemyMask;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (isPassive)
        {
            Destroy(gameObject);
            return;
        }

        if (target == null || card == null)
            return;

        float duration = card.activeTime;
        float value = card.effectValue;

        Vector2 center = target.transform.position;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(center, radius, enemyMask);

        foreach (var enemyCol in enemies)
        {
            EnemyStatus status = enemyCol.GetComponent<EnemyStatus>();
            if (status != null)
            {
                var effect = new StatusEffect(effectType, duration, value, 1f, 1);
                status.ApplyEffect(effect);
            }
        }

        Destroy(gameObject, duration + 0.5f);
    }

    public void Deactivate() {}
    public void Tick(float deltaTime) {}
} 
