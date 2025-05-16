using UnityEngine;

public class SplitFireModifier : MonoBehaviour, IBulletModifier
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int numberOfSplits = 2;
    [SerializeField] private float splitAngle = 30f;

    public void Apply(BulletBase bullet, Enemy target)
    {
        if (bulletPrefab == null || numberOfSplits < 1) return;

        Vector2 forward = bullet.transform.right;
        float baseAngle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - splitAngle * 0.5f * (numberOfSplits - 1);

        for (int i = 0; i < numberOfSplits; i++)
        {
            float angle = startAngle + splitAngle * i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject newBullet = Instantiate(bulletPrefab, bullet.transform.position, Quaternion.identity);
            BulletBase split = newBullet.GetComponent<BulletBase>();
            if (split != null)
            {
                split.Configure(bullet.rangedWeapon);
                split.Shoot(bullet.damage, direction, false);
            }
        }
    }
}