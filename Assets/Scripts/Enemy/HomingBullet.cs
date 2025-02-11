using UnityEngine;

public class HomingBullet : EnemyBullet
{
    [Header("Homing Bullet Settings")]
    [SerializeField] private float pauseDuration = 0.5f;  // Time before moving
    [SerializeField] private float speed = 12f;           // Speed after launching

    private Vector2 targetPosition;
    private bool isLockedOn = false;
    private Rigidbody2D rb;
    private float currentPauseDuration;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPauseDuration = pauseDuration;
    }

    private void OnEnable()
    {
        rb.linearVelocity = Vector2.zero;
        isLockedOn = false;
    }

    private void LockOnAndFire()
    {
        GameObject player = CharacterManager.Instance.gameObject;

        if (player != null)
        {
            targetPosition = player.transform.position;
            isLockedOn = true;
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private void Update()
    {
        if (pauseDuration >= 0)
            pauseDuration -= Time.deltaTime;    
        else
            LockOnAndFire();

        if(isLockedOn)
           MoveTowardsPlayer();
    }
    

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            collider.GetComponent<CharacterManager>().TakeDamage(10); // Apply damage
            Destroy(gameObject); // Destroy on hit
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = moveDirection * speed;

        if (Vector2.Distance(transform.position, targetPosition) < 0.2f)
        {
            Destroy(gameObject);
        }
    }
}
