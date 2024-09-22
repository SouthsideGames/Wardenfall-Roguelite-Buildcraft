using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyBullet : MonoBehaviour
{

    [Header("ELEMENTS:")]
    private Rigidbody2D rb;
    private Collider2D col;
    private RangedEnemyAttack rangedEnemyAttack;


    [Header("SETTINGS:")]
    [SerializeField] private float moveSpeed;
    private int damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();   

        LeanTween.delayedCall(gameObject, 5, () => rangedEnemyAttack.ReleaseBullet(this));
    }

    public void Configure(RangedEnemyAttack _rangedEnemyAttack) => rangedEnemyAttack = _rangedEnemyAttack;

    public void Shoot(int _damage, Vector2 _direction)
    {
        this.damage = _damage;  

        transform.right = _direction;
        rb.velocity = _direction * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the bullet hits the player
        if (collider.TryGetComponent(out CharacterManager player))
        {

            LeanTween.cancel(gameObject);

            player.TakeDamage(damage);
            col.enabled = false;    
            rangedEnemyAttack.ReleaseBullet(this);
        }
        
    }

    public void Reload()
    {
        rb.velocity = Vector2.zero;

        col.enabled = true; 

    }
}
