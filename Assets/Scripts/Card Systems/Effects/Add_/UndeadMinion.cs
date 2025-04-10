using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UndeadMinion : MonoBehaviour
{
    [Header("SETTINGS:")]
    [SerializeField] private float lifetime;
    [SerializeField] private float explosionRadius;

    private bool hasExploded = false;
    private float explosionDamage;


    public void InitializeMinion(float _lifetime, CardSO _card)
    {
        lifetime = _lifetime;
        explosionDamage = _card.EffectValue;
        CoroutineRunner.Instance.RunPooled(LifetimeCountdown());
    }

    private IEnumerator LifetimeCountdown()
    {
        yield return new WaitForSeconds(lifetime);
        Explode();
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Deal damage to enemies in the explosion radius
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Enemy"));
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)explosionDamage, false);
            }
        }

        // Show explosion effect (optional)
        Debug.Log($"Minion exploded dealing {explosionDamage} AoE damage.");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
