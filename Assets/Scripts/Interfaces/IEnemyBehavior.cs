
public interface IEnemyBehavior
{
    void Initialize(EnemyDataSO data);
    void UpdateBehavior();
    void OnHit();
    void ApplyEffect(StatusEffect effect);
}
