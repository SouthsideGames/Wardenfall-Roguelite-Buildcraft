using UnityEngine;

public class EnergyWave : BulletBase
{
  private float speed;
    private int lifeStealAmount;
    private Vector2 direction;
    private System.Action onWaveComplete; // Callback for completion

    public void Initialize(int _damage, float _speed, LayerMask _enemyMask, int _lifeStealAmount, System.Action _onWaveComplete, Vector2 _direction)
    {
        damage = _damage;
        speed = _speed;
        enemyMask = _enemyMask;
        lifeStealAmount = _lifeStealAmount;
        onWaveComplete = _onWaveComplete;
        direction = _direction.normalized; // Normalize direction vector

        // Automatically destroy after 3 seconds
        Invoke(nameof(DestroyWave), 3f); 
    }

    private void Update()
    {
        // Move the wave in the given direction
        transform.Translate(direction * speed * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if collided object is an enemy
        if (((1 << collider.gameObject.layer) & enemyMask) != 0)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, false);
                HealPlayer(); // Life steal
            }

            CompleteWave();
        }
    }

    private void HealPlayer()
    {
        CharacterManager.Instance.health.Heal(lifeStealAmount);
    }

    private void CompleteWave()
    {
        // Notify weapon to allow the next wave
        onWaveComplete?.Invoke();
        Destroy(gameObject); // Destroy the wave
    }

    private void DestroyWave()
    {
        // Notify weapon if wave times out
        onWaveComplete?.Invoke();
        Destroy(gameObject);
    }
}
