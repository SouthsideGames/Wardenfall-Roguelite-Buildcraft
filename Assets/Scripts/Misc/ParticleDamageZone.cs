using UnityEngine;

public class ParticleDamageZone : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageInterval = 1f; // Time between damage ticks

    private ParticleSystem particle;
    private float nextDamageTime;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            nextDamageTime = Time.time + damageInterval;
            CharacterManager.Instance.health.TakeDamage(damageAmount);
        }
    }
}
