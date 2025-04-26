using UnityEngine;

public class ToxicPulseEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private float effectDuration = 4f;
    [SerializeField] private float effectValue = 5f;

    public void Activate(CharacterManager target, CardSO card)
    {
        StatusEffectType[] effectPool = new[]
        {
            StatusEffectType.Burn,
            StatusEffectType.Freeze,
            StatusEffectType.Drain,
            StatusEffectType.Slow,
            StatusEffectType.Paralyze,
            StatusEffectType.Weaken
        };

        StatusEffectType selectedType = effectPool[Random.Range(0, effectPool.Length)];

        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            EnemyStatus status = enemy.GetComponent<EnemyStatus>();
            if (status != null)
            {
                StatusEffect effect = new StatusEffect(selectedType, effectDuration, effectValue);
                status.ApplyEffect(effect);
            }
        }

        Debug.Log($"Toxic Pulse activated: Applied {selectedType} to all enemies.");
        Destroy(gameObject);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
