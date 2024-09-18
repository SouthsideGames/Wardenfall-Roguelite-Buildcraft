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
    private RangedWeapon rangedWeapon;

    [Header("Setting")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float moveSpeed;
    private int damage;
    private bool isCriticalHit;
    private Enemy target;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();   

        //LeanTween.delayedCall(gameObject, 5, () => rangedEnemyAttack.ReleaseBullet(this));
    }

    public void Configure(RangedWeapon _rangedWeapon)
    {
        this.rangedWeapon = _rangedWeapon;
    }

    public void Shoot(int _damage, Vector2 _direction, bool _isCriticalHit)
    {
        Invoke("Release", 1);

        damage = _damage;  
        isCriticalHit = _isCriticalHit;

        transform.right = _direction;
        rb.velocity = _direction * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(target != null)
            return;

        // Check if the bullet hits the player
        if (IsInLayerMask(collider.gameObject.layer, enemyMask))
        {

            target = collider.GetComponent<Enemy>();

            CancelInvoke();

            Attack(target);
            Release();

        }
        
    }

    private void Release()
    {
        if(!gameObject.activeSelf)
           return;

        rangedWeapon.ReleaseBullet(this);
    }

    private void Attack(Enemy _enemy)
    {
        _enemy.TakeDamage(damage, isCriticalHit);
    }

    private bool IsInLayerMask(int _layer, LayerMask _layerMask)
    {
        return (_layerMask.value & (1 << _layer)) != 0; 
    }

     public void Reload()
    {
        target = null;

        rb.velocity = Vector2.zero;
        col.enabled = true; 

    }


}
