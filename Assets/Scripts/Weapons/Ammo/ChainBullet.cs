using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ChainBullet : BulletBase
{
    [Header("CHAIN SETTINGS")]
    [SerializeField] private float chainRadius;
    [SerializeField] private int maxChains;
    [SerializeField] private float damageFalloff;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float chainEffectDuration = 0.2f;

    private List<Enemy> hitEnemies = new List<Enemy>();
    private LineRenderer lineRenderer;

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    public void Initialize(int _damage, Vector2 direction, bool _isCriticalHit, float _chainRadius, int _maxChains, float _damageFalloff, LayerMask _enemyMask)
    {
        base.Shoot(_damage, direction, _isCriticalHit);
        chainRadius = _chainRadius;
        maxChains = _maxChains;
        damageFalloff = _damageFalloff;
        enemyMask = _enemyMask;

        ChainToTarget(transform.position, _damage, 0);

        StartCoroutine(ClearChainEffect());
        DestroyBullet();
    }

    private void ChainToTarget(Vector2 position, int currentDamage, int chainCount)
    {
        if (chainCount >= maxChains || currentDamage <= 0)
            return;

        Enemy nextTarget = GetClosestTarget(position);

        if (nextTarget == null || hitEnemies.Contains(nextTarget))
            return;

        ApplyDamage(nextTarget);

        hitEnemies.Add(nextTarget);

        AddChainEffect(position, nextTarget.transform.position);

        ChainToTarget(nextTarget.transform.position, Mathf.RoundToInt(currentDamage * damageFalloff), chainCount + 1);
    }

    private Enemy GetClosestTarget(Vector2 origin)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(origin, chainRadius, enemyMask);
        Enemy closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                float distance = Vector2.Distance(origin, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closest = enemy;
                    closestDistance = distance;
                }
            }
        }

        return closest;
    }

    private void AddChainEffect(Vector2 start, Vector2 end)
    {
        int positionCount = lineRenderer.positionCount;
        lineRenderer.positionCount += 2;

        lineRenderer.SetPosition(positionCount, start);
        lineRenderer.SetPosition(positionCount + 1, end);
    }

    private System.Collections.IEnumerator ClearChainEffect()
    {
        yield return new WaitForSeconds(chainEffectDuration);
        lineRenderer.positionCount = 0;
        hitEnemies.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chainRadius);
    }
}
