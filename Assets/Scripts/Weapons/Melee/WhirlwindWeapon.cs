using UnityEngine;

public class WhirlwindWeapon : MeleeWeapon
{
    private MeleeWeaponState state;

    [Header("SPINNY SETTINGS")]
    [SerializeField] private float orbitRadius = 2f;
    [SerializeField] private float orbitSpeed = 50f;

    private Transform centerObject;
    private float angle;

    private void Start()
    {
        centerObject = FindAnyObjectByType<CharacterManager>().gameObject.transform;
    }

    void Update()
    {
        anim.Play("Attack");

        if (centerObject == null) return;

        angle += orbitSpeed * Time.deltaTime;

        if (angle >= 360f)
        {
            angle -= 360f;
            damagedEnemies.Clear();
        }

        float angleRad = angle * Mathf.Deg2Rad;

        float x = centerObject.position.x + Mathf.Cos(angleRad) * orbitRadius;
        float y = centerObject.position.y + Mathf.Sin(angleRad) * orbitRadius;

        transform.position = new Vector2(x, y);

        DamageEnemies();
    }

    private void DamageEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapBoxAll(
            transform.position,
            hitCollider.bounds.size,
            0f,
            enemyMask
        );

        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && !damagedEnemies.Contains(enemy))
            {
                int damage = GetDamage(out bool isCriticalHit);
                enemy.TakeDamage(damage, isCriticalHit);
                damagedEnemies.Add(enemy);
            }
        }
    }

}
