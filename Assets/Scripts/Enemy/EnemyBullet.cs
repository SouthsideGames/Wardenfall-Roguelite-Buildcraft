using UnityEngine;
using System;

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
    [SerializeField] private float angularSpeed;
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
        damage = _damage;  

        if(Mathf.Abs(_direction.x + 1) < 0.01f)
            _direction.y += .01f;

        transform.right = _direction;
        rb.linearVelocity = _direction * moveSpeed;
        rb.angularVelocity = angularSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out CharacterManager player))
        {
            player.TakeDamage(damage);
            col.enabled = false;    
            rangedEnemyAttack.ReleaseBullet(this);
        }
        else if (collider.TryGetComponent(out SurvivorBox box))
        {
            box.TakeDamage(damage);
            col.enabled = false;
            rangedEnemyAttack.ReleaseBullet(this);
        }
        
    }

    public void Reload()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        col.enabled = true; 

        LeanTween.cancel(gameObject);
        LeanTween.delayedCall(gameObject, 5, () => rangedEnemyAttack.ReleaseBullet(this));
    }
}
