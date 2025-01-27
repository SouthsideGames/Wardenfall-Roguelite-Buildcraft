using System.Collections;
using UnityEngine;

public class EnergyLinkEffect : ICardEffect
{
     private Transform playerTransform;
    private Transform enemyTransform;
    private LineRenderer linkRenderer;
    private int damagePerTick;
    private float tickInterval = 1f;
    private float activeTime;

    public EnergyLinkEffect(Transform playerTransform, LineRenderer linkRenderer, int damagePerTick)
    {
        this.playerTransform = playerTransform;
        this.linkRenderer = linkRenderer;
        this.damagePerTick = damagePerTick;
    }

    public void Activate(float duration)
    {
        activeTime = duration;
        Enemy targetEnemy = FindClosestEnemy();

        if (targetEnemy != null)
        {
            enemyTransform = targetEnemy.transform;
            CoroutineRunner.Instance.StartCoroutine(LinkEnemy(targetEnemy));
        }
        else
        {
            Debug.LogWarning("No enemy found for Energy Link!");
        }
    }

    public void Disable()
    {
        if (linkRenderer != null)
            linkRenderer.enabled = false;

        Debug.Log("Energy Link effect disabled.");
    }

    private IEnumerator LinkEnemy(Enemy enemy)
    {
        float elapsedTime = 0f;

        linkRenderer.enabled = true;

        while (elapsedTime < activeTime && enemy != null && enemy.isActiveAndEnabled)
        {
            // Draw the energy link
            linkRenderer.SetPosition(0, playerTransform.position);
            linkRenderer.SetPosition(1, enemy.transform.position);

            // Steal health
            int damage = Mathf.FloorToInt(damagePerTick);
            enemy.TakeDamage(damage, false);
            CharacterManager.Instance.health.Heal(damage);

            elapsedTime += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }

        Disable(); // Clean up after the effect ends
    }

    private Enemy FindClosestEnemy()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(playerTransform.position, 15f, LayerMask.GetMask("Enemy"));
        Enemy closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in enemiesInRange)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            float distance = Vector2.Distance(playerTransform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }

        return closestEnemy;
    }
}
