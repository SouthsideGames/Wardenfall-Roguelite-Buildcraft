using UnityEngine;

public class QuantumPullEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private float pullRadius = 100f;
    [SerializeField] private float pullForce = 50f;
    [SerializeField] private LayerMask collectibleMask;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null)
        {
            Debug.LogWarning("QuantumPullEffect: Target is null.");
            return;
        }

        Vector2 playerPos = target.transform.position;
        Collider2D[] items = Physics2D.OverlapCircleAll(playerPos, pullRadius, collectibleMask);

        foreach (Collider2D item in items)
        {
            Rigidbody2D rb = item.attachedRigidbody;
            if (rb != null)
            {
                Vector2 direction = (playerPos - rb.position).normalized;
                rb.linearVelocity = direction * pullForce;
            }
        }

        Debug.Log($"Quantum Pull activated: pulled {items.Length} items.");
        Destroy(gameObject);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
