
public interface IDamageable
{
    void TakeDamage(float damage, bool isCritical = false);
    void Die();
    bool IsAlive { get; }
    float CurrentHealth { get; }
    float MaxHealth { get; }
}
