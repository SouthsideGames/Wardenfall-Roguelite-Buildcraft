using System.Collections;
using UnityEngine;

public class FireWave : MonoBehaviour
{
    private float speed;
    private float range;
    private float damage;
    private float burnDamage;
    private float burnDuration;
    private bool isCriticalHit;
    private Vector2 startPosition;

    [Header("ELEMENTS:")]
    [SerializeField] private LayerMask enemyMask; // Detect enemies
    [SerializeField] private ParticleSystem fireEffect; // Fire VFX
    [SerializeField] private Collider2D hitCollider; // Damage area

    public void Setup(float _damage, float _speed, float _range, float _burnDamage, float _burnDuration, float _width, bool _isCriticalHit)
    {
        damage = _damage;
        speed = _speed;
        range = _range;
        burnDamage = _burnDamage;
        burnDuration = _burnDuration;
        isCriticalHit = _isCriticalHit;

        startPosition = transform.position;

        // Adjust the hitbox width for the wave
        transform.localScale = new Vector3(_width, 1, 1);

        Destroy(gameObject, range / speed); // Destroy after traveling the range
    }

    private void Update()
    {
        // Move the wave forward
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        // Check if the wave has reached its range
        if (Vector2.Distance(startPosition, transform.position) >= range)
        {
            Destroy(gameObject); // Destroy once out of range
        }

        // Detect enemies in the path
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(transform.position, hitCollider.bounds.size, 0, enemyMask);

        foreach (var enemyCollider in hitEnemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Apply initial damage
                enemy.TakeDamage(Mathf.RoundToInt(damage), isCriticalHit);

                // Apply burn effect
                StartCoroutine(ApplyBurn(enemy));
            }
        }
    }

    IEnumerator ApplyBurn(Enemy enemy)
    {
        float timer = 0;

        while (timer < burnDuration && enemy.health > 0)
        {
            yield return new WaitForSeconds(1f);
            enemy.TakeDamage(Mathf.RoundToInt(burnDamage), false);
            timer += 1f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, hitCollider.bounds.size);
    }
}
