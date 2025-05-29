using System.Collections;
using UnityEngine;

public class VoidWarden : MonoBehaviour
{
    [SerializeField] private float auraRadius = 4f;
    [SerializeField] private GameObject auraVisual;
    [SerializeField] private StatusEffect weakenEffect;

    private float activeTime;

    public void Initialize(float duration)
    {
        activeTime = duration;
        if (auraVisual != null)
            auraVisual.transform.localScale = Vector3.one * auraRadius * 2f;

        StartCoroutine(DebuffAuraRoutine());
        Destroy(gameObject, activeTime);
    }

    private IEnumerator DebuffAuraRoutine()
    {
        float tickRate = 0.5f;
        while (true)
        {
            ApplyDebuffToEnemies();
            yield return new WaitForSeconds(tickRate);
        }
    }

    private void ApplyDebuffToEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, auraRadius);
        foreach (var hit in hits)
        {
            EnemyStatus status = hit.GetComponent<EnemyStatus>();
            if (status != null)
            {
                status.ApplyEffect(weakenEffect, 0f);
            }
        }
    }
}
