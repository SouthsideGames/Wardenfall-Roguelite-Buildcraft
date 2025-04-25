using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRingZone : MonoBehaviour
{
    [SerializeField] private float radius = 2.5f;
    [SerializeField] private float tickInterval = 1f;
    [SerializeField] private LayerMask enemyMask;

    private float damagePerTick;
    private float activeDuration;

    public void Initialize(float damage, float duration)
    {
        damagePerTick = damage;
        activeDuration = duration;

        // Maintain custom Y position and rotation offsets
        Vector3 pos = transform.localPosition;
        pos.y = 0.5f;
        transform.localPosition = pos;
        transform.localEulerAngles = new Vector3(0f, 90f, -90f);

        StartCoroutine(ApplyDamageOverTime());
    }

    private IEnumerator ApplyDamageOverTime()
    {
        float elapsed = 0f;

        while (elapsed < activeDuration)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
            foreach (Collider2D enemyCollider in enemies)
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage((int)damagePerTick);
                }
            }

            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
} 
