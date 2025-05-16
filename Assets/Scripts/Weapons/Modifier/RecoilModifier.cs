using UnityEngine;

public class RecoilModifier : MonoBehaviour, IBulletModifier
{
    [SerializeField] private float knockbackForce = 4f;
    [SerializeField] private float knockbackDuration = 0.15f;

    public void Apply(BulletBase bullet, Enemy target)
    {
        if (target != null)
        {
            Vector2 direction = (target.transform.position - bullet.transform.position).normalized;
            target.movement.ApplyKnockback(direction, knockbackForce, knockbackDuration);
        }
    }
}