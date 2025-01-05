using UnityEngine;

public class EnergyWave : MonoBehaviour
{
    private float speed;
    private int lifeStealAmount;
    private Vector2 direction;
    private int damage;
    private LayerMask enemyMask;
    private System.Action onWaveComplete;

    public void Initialize(int _damage, float _speed, LayerMask _enemyMask, int _lifeStealAmount, System.Action _onWaveComplete, Vector2 _direction)
    {
        damage = _damage;
        speed = _speed;
        enemyMask = _enemyMask;
        lifeStealAmount = _lifeStealAmount;
        onWaveComplete = _onWaveComplete;
        direction = _direction.normalized; // Normalize direction vector
        Invoke(nameof(DestroyWave), 3f); 
    }

    private void Update()
    {
        
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        
        if (((1 << collider.gameObject.layer) & enemyMask) != 0)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, false);
                HealPlayer(); 
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
      
        onWaveComplete?.Invoke();
        Destroy(gameObject); 
    }

    private void DestroyWave()
    {
        onWaveComplete?.Invoke();
        Destroy(gameObject);
    }
}
