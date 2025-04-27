using UnityEngine;


public abstract class EnvironmentalHazard : MonoBehaviour
{
    [Header("Hazard Settings")]
    [SerializeField] protected int damageAmount = 10;
    [SerializeField] protected float damageInterval = 1f; 
    [SerializeField] protected float lifetime = -1f; 

    protected float nextDamageTime;


    protected virtual void Start()
    {
        
    }

    protected virtual void OnTriggerStay2D(Collider2D other) => FindTarget(other);

    private void FindTarget(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            ApplyHazardEffect(other);
            nextDamageTime = Time.time + damageInterval;
        }
        else if (other.CompareTag("Enemy") && Time.time >= nextDamageTime)
        {
            ApplyHazardEffect(other);
            nextDamageTime = Time.time + damageInterval;
        }
    }

    protected virtual void ApplyHazardEffect(Collider2D other) => DamageTarget(other);
    private void DamageTarget(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterManager.Instance.health.TakeDamage(damageAmount);
        }
        else if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy?.TakeDamage(damageAmount);
        }
    }
}