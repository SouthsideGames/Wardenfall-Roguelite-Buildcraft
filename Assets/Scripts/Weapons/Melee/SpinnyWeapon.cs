using UnityEngine;

public class SpinnyWeapon : MeleeWeapon
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

        float angleRad = angle * Mathf.Deg2Rad;

        float x = centerObject.position.x + Mathf.Cos(angleRad) * orbitRadius;
        float y = centerObject.position.y + Mathf.Sin(angleRad) * orbitRadius;

        transform.position = new Vector2(x, y);

        DamageEnemies();
    }

    private void DamageEnemies()
    {
        // Use a collider to detect enemies in range
        Collider2D[] enemies = Physics2D.OverlapBoxAll(
            transform.position,
            hitCollider.bounds.size,  // Use the size of the collider
            0f,                       // No rotation needed
            enemyMask                 // The layer mask for enemies
        );

        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && !damagedEnemies.Contains(enemy))
            {
                // Deal damage to the enemy
                int damage = GetDamage(out bool isCriticalHit);
                enemy.TakeDamage(damage, isCriticalHit);

                // Add enemy to the damaged list to avoid repeated damage
                damagedEnemies.Add(enemy);
            }
        }

        // Clear the damaged list to allow enemies to be hit again after some time
        if (state == MeleeWeaponState.Idle)
        {
            damagedEnemies.Clear();
        }
    }

}
