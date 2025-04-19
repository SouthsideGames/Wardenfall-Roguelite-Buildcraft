using UnityEngine;

public class SplitterBullet : BulletBase
{
    [Header("SPLITTER SETTINGS")]
    [SerializeField] private BulletBase splitBulletPrefab;
    [SerializeField] private float splitDamageMultiplier = 0.5f;
    [SerializeField] private float maxDistance = 10f;

    [Header("SPLIT SPAWN POINTS")]
    [SerializeField] private Transform[] spawnPoints;

    private Vector2 startPosition;

    protected override void Awake()
    {
        base.Awake();
        startPosition = transform.position;
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, startPosition) >= maxDistance)
        {
            TriggerSplit();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsInLayerMask(collider.gameObject.layer, enemyMask))
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                ApplyDamage(enemy); 
            }

            TriggerSplit();
        }
    }

    private void TriggerSplit()
    {
        SpawnSplitBullets();
        DestroyBullet();
        SplitterWeapon.OnAmmoAFinished?.Invoke(); 
    }

    private void SpawnSplitBullets()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            BulletBase splitBullet = Instantiate(splitBulletPrefab, spawnPoint.position, spawnPoint.rotation);
            splitBullet.Shoot(Mathf.RoundToInt(damage * splitDamageMultiplier), Vector2.zero, isCriticalHit); 
        }
    }
}
