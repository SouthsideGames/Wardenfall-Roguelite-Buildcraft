using UnityEngine;

public class QuantumBarrageEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int projectileCount = 24;
    [SerializeField] private float projectileSpeedOverride = 8f;

    private int damage;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || projectilePrefab == null)
        {
            Debug.LogWarning("QuantumBarrageEffect: Target or prefab is missing.");
            return;
        }

        damage = Mathf.RoundToInt(card.effectValue > 0 ? card.effectValue : 10f);
        FireRadialBurst(target.transform.position);
        Destroy(gameObject); 
    }

    public void Deactivate() { }

    public void Tick(float deltaTime) { }

    private void FireRadialBurst(Vector3 origin)
    {
        float angleStep = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

            GameObject proj = Instantiate(projectilePrefab, origin, Quaternion.identity);
            if (proj.TryGetComponent(out BulletBase bullet))
            {
                bullet.Configure(null); 
                bullet.Shoot(damage, direction, false);

                var rb = proj.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.linearVelocity = direction * projectileSpeedOverride;
            }
        }
    }
}
