using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [Header("Element")]
    private Rigidbody2D rb;
    private Collider2D col;

    [Header("Setting")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float moveSpeed;
    private int damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();   

        //LeanTween.delayedCall(gameObject, 5, () => rangedEnemyAttack.ReleaseBullet(this));
    }

    public void Shoot(int _damage, Vector2 _direction)
    {
        this.damage = _damage;  

        transform.right = _direction;
        rb.velocity = _direction * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the bullet hits the player
        if (IsInLayerMask(collider.gameObject.layer, enemyMask))
        {
            Attack(collider.GetComponent<Enemy>());
            Destroy(gameObject);

        }
        
    }

    private void Attack(Enemy _enemy)
    {
        _enemy.TakeDamage(damage);
    }

    private bool IsInLayerMask(int _layer, LayerMask _layerMask)
    {
        return (_layerMask.value & (1 << _layer)) != 0; 
    }


}
