using UnityEngine;

public class SplitterBullet : BulletBase
{
    [Header("SPLITTER SETTINGS")]
    [SerializeField] private BulletBase splitBulletPrefab;
    [SerializeField] private float splitDamageMultiplier = 0.5f;

    private readonly Vector2[] directions =
    {
        Vector2.up,               // N
        new Vector2(1, 1).normalized,  // NE
        Vector2.right,            // E
        new Vector2(1, -1).normalized, // SE
        Vector2.down,             // S
        new Vector2(-1, -1).normalized, // SW
        Vector2.left,             // W
        new Vector2(-1, 1).normalized   // NW
    };

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsInLayerMask(collider.gameObject.layer, enemyMask))
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                ApplyDamage(enemy);
            }

            SpawnSplitBullets();
            DestroyBullet();
        }
    }

    private void SpawnSplitBullets()
    {
        foreach (Vector2 direction in directions)
        {
            BulletBase splitBullet = Instantiate(splitBulletPrefab, transform.position, Quaternion.identity);
            splitBullet.Shoot(Mathf.RoundToInt(damage * splitDamageMultiplier), Vector2.zero, isCriticalHit);

            // Stop movement to make the split bullets stationary
            splitBullet.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }
}
