using UnityEngine;

public class FlameBullet : BulletBase
{
    private GameObject fireWallPrefab;
    private int fireWallDamage;
    private float fireWallDuration;

    public void SetupFireWall(GameObject prefab, int damage, float duration)
    {
        fireWallPrefab = prefab;
        fireWallDamage = damage;
        fireWallDuration = duration;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (target != null)
            return;

        if (IsInLayerMask(collision.gameObject.layer, enemyMask))
        {
            target = collision.GetComponent<Enemy>();

            if (target != null)
            {
                CancelInvoke();
                ApplyDamage(target);
                CreateFireWall(target.transform.position);
                Release();
            }
        }
    }

    private void CreateFireWall(Vector2 position)
    {
        GameObject fireWall = Instantiate(fireWallPrefab, position, Quaternion.identity);
        FireWall fireWallScript = fireWall.GetComponent<FireWall>();
        
        if (fireWallScript != null)
        {
            fireWallScript.Setup(fireWallDamage, fireWallDuration);
        }
    }
}
