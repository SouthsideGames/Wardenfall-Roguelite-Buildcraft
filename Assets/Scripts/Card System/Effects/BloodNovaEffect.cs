using UnityEngine;

public class BloodNovaEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private float baseRadius = 4f;
    [SerializeField] private float baseDamage = 20f;
    [SerializeField] private float maxMultiplier = 3f;
    [SerializeField] private LayerMask enemyMask;

    public void Activate(CharacterManager _cm, CardSO card)
    {
        if (_cm == null || _cm.health == null)
            return;

        float currentHealth = _cm.health.CurrentHealth;
        float maxHealth = _cm.health.MaxHealth;
        float missingPercent = Mathf.Clamp01(1f - (currentHealth / maxHealth));

        float finalDamage = baseDamage * (1f + (missingPercent * (maxMultiplier - 1f)));
        float finalRadius = baseRadius * (1f + missingPercent);

        Collider2D[] enemies = Physics2D.OverlapCircleAll(_cm.transform.position, finalRadius, enemyMask);
        foreach (Collider2D col in enemies)
        {
            if (col.TryGetComponent(out Enemy enemy))
                enemy.TakeDamage((int)finalDamage);
        }

        Destroy(gameObject);
    }

    public void Deactivate() { }

    public void Tick(float deltaTime) { }
}
