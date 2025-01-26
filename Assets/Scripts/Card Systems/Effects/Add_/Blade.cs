using UnityEngine;

public class Blade : MonoBehaviour
{
    private float rotationSpeed = 360f;

    [Tooltip("Detection layer for enemies.")]
    [SerializeField] private LayerMask enemyMask;

    private float damage;
    private Collider2D[] colliders;
    private CardSO cardSO;

    public void Configure(CardSO _card)
    {
        this.cardSO = _card;
    }


    private void Start()
    {
        damage = cardSO.EffectValue;


        colliders = GetComponentsInChildren<Collider2D>();

        if (colliders.Length == 0)
        {
            Debug.LogWarning("No colliders found on Blade or its children.");
        }
    }

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProcessCollision(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision.collider);
    }

    private void ProcessCollision(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & enemyMask) != 0)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)damage, false);
            }
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
