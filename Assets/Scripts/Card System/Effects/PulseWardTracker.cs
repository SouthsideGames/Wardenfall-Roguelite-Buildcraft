using UnityEngine;

public class PulseWardTracker : MonoBehaviour
{
    [SerializeField] private GameObject burstEffectPrefab;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float cooldown = 8f;
    [SerializeField] private float burstRadius = 3f;
    [SerializeField] private LayerMask enemyMask;

    private float cooldownTimer = 0f;
    private CharacterManager character;

    private void Awake() => character = GetComponent<CharacterManager>();

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public void OnDamageTaken()
    {
        if (!character.cards.HasCard("S-025")) return;
        if (cooldownTimer > 0f) return;

        TriggerPulse();
        cooldownTimer = cooldown;
    }

    private void TriggerPulse()
    {
        if (burstEffectPrefab != null)
            Instantiate(burstEffectPrefab, character.transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(character.transform.position, burstRadius, enemyMask);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage((int)damage);
        }

        Debug.Log("Pulse Ward triggered: AoE burst applied.");
    }
}
