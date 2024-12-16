using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainWeapon : RangedWeapon
{ 
    [Header("CHAIN WEAPON SPECIFICS:")]
    [SerializeField] private int maxBounces;
    [SerializeField] private float bounceRange;
    [SerializeField] private GameObject lightningBoltPrefab; // Reference to the lightning visual effect prefab
    [SerializeField] private float boltDuration = 0.1f;      // Duration each bolt is visible

    protected override void Shoot()
    {
        ChainBolt(transform.position, damage, maxBounces);
    }

    private void ChainBolt(Vector2 position, int damage, int remainingBounces)
    {
        if (remainingBounces <= 0) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(position, bounceRange, enemyMask);
        if (enemies.Length == 0) return;

        Enemy target = enemies[Random.Range(0, enemies.Length)].GetComponent<Enemy>();
        target.TakeDamage(damage, _isCriticalHit: false);

        // Create lightning bolt effect between current position and target
        CreateLightningBolt(position, target.transform.position);

        // Recursively call ChainBolt on the next target
        ChainBolt(target.transform.position, damage, remainingBounces - 1);
    }

    private void CreateLightningBolt(Vector2 start, Vector2 end)
    {
        GameObject bolt = Instantiate(lightningBoltPrefab);
        LineRenderer lineRenderer = bolt.GetComponent<LineRenderer>();

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // Destroy the bolt after a short duration
        StartCoroutine(FadeLightningBolt(lineRenderer));
    }

    private IEnumerator FadeLightningBolt(LineRenderer lineRenderer)
    {
        float elapsed = 0f;
        Color startColor = lineRenderer.startColor;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < boltDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / boltDuration);
            lineRenderer.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            lineRenderer.endColor = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(lineRenderer.gameObject);
    }

}
