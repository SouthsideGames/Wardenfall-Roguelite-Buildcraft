using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyBullet : MonoBehaviour
{

    [Header("ELEMENTS:")]
    protected Rigidbody2D rb;
    private Collider2D col;
    private RangedEnemyAttack rangedEnemyAttack;

    [Header("SETTINGS:")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float angularSpeed;
    protected int damage;

    private bool isReleased = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Configure(RangedEnemyAttack _rangedEnemyAttack)
    {
        rangedEnemyAttack = _rangedEnemyAttack;
        isReleased = false; 
    }

    public void Shoot(int _damage, Vector2 _direction)
    {
        damage = _damage;

        if (Mathf.Abs(_direction.x + 1) < 0.01f)
            _direction.y += .01f;

        transform.right = _direction;
        rb.linearVelocity = _direction * moveSpeed;
        rb.angularVelocity = angularSpeed;

        LeanTween.cancel(gameObject);
        LeanTween.delayedCall(gameObject, 5, () => ReleaseBullet());
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out CharacterManager player))
        {
            player.TakeDamage(damage);
            col.enabled = false;
            ReleaseBullet();
        }
    }

    public void Reload()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        col.enabled = true;

        LeanTween.cancel(gameObject);

        LeanTween.delayedCall(gameObject, 5, () => ReleaseBullet());

        isReleased = false;
    }

    protected void ReleaseBullet()
    {
        if (!isReleased) 
        {
            isReleased = true; 
            rangedEnemyAttack.ReleaseBullet(this); 
        }
    }
}
