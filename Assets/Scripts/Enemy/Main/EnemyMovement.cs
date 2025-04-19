using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("ELEMENTS:")]
    private Transform currentTarget;
    private Vector2 targetPosition;
    private bool isTargetPositionSet = false;

    [Header("SETTINGS:")]
    public float moveSpeed;
    private bool canMove = true;
    private bool isKnockedBack = false;

    private Vector2 knockbackDirection;
    private float knockbackSpeed = 0f;

    [SerializeField] private float pathUpdateRate = 0.2f;
    [SerializeField] private float avoidanceRadius = 1f;
    private float pathUpdateTimer;
    private Vector2 currentDirection;
    private Rigidbody2D rb; // Added Rigidbody2D component


    public void StorePlayer(CharacterManager _player) => currentTarget = _player.transform;

    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
        isTargetPositionSet = false; // Disable target position when following a transform
    }

    public void SetTargetPosition(Vector2 newPosition)
    {
        targetPosition = newPosition;
        isTargetPositionSet = true; // Enable movement toward a fixed position
    }

    public void FollowCurrentTarget()
    {
        if (currentTarget == null || rb == null) return; // Check for Rigidbody2D

        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateRate)
        {
            UpdatePath();
            pathUpdateTimer = 0;
        }

        // Apply current movement with obstacle avoidance
        Vector2 avoidanceForce = CalculateAvoidanceForce();
        Vector2 finalDirection = (currentDirection + avoidanceForce).normalized;
        rb.velocity = finalDirection * moveSpeed;
    }

    private void UpdatePath()
    {
        if (currentTarget == null) return;

        // Update direction with basic obstacle checking
        Vector2 targetPos = currentTarget.position;
        Vector2 directionToTarget = (targetPos - (Vector2)transform.position).normalized;

        // Raycast to check for obstacles
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, 5f);
        if (hit.collider != null && !hit.collider.CompareTag("Player"))
        {
            // If obstacle found, try to find alternative path
            Vector2 alternativePath = FindAlternativePath(targetPos);
            currentDirection = alternativePath;
        }
        else
        {
            currentDirection = directionToTarget;
        }
    }

    private Vector2 CalculateAvoidanceForce()
    {
        Vector2 avoidanceForce = Vector2.zero;
        List<Enemy> nearbyEnemies = new List<Enemy>(); //Initialized here

        // Get nearby enemies
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<Enemy>(out var enemy) && enemy != this)
            {
                nearbyEnemies.Add(enemy);
            }
        }

        // Calculate avoidance
        foreach (var enemy in nearbyEnemies)
        {
            Vector2 awayFromEnemy = (Vector2)(transform.position - enemy.transform.position).normalized;
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            avoidanceForce += awayFromEnemy * (avoidanceRadius - distance) / avoidanceRadius;
        }

        return avoidanceForce.normalized;
    }

    private Vector2 FindAlternativePath(Vector2 targetPos)
    {
        // Try multiple angles to find clear path
        float[] testAngles = { 45f, -45f, 90f, -90f };
        Vector2 originalDirection = (targetPos - (Vector2)transform.position).normalized;

        foreach (float angle in testAngles)
        {
            Vector2 testDirection = RotateVector(originalDirection, angle);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, testDirection, 3f);

            if (hit.collider == null)
            {
                return testDirection;
            }
        }

        return originalDirection;
    }

    private Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }

    public void MoveToTargetPosition()
    {
        if (!canMove || !isTargetPositionSet || isKnockedBack) return;

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            isTargetPositionSet = false; // Reset to allow new movement
        }
    }

    public void DisableMovement(float duration)
    {
        CoroutineRunner.Instance.RunPooled(DisableMovementTemporarily(duration));
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    private IEnumerator DisableMovementTemporarily(float duration)
    {
        canMove = false;
        yield return new WaitForSeconds(duration);
        canMove = true;
    }

    public void SetRunAwayFromPlayer()
    {
        if (currentTarget != null)
        {
            Vector2 direction = ((Vector2)transform.position - (Vector2)currentTarget.position).normalized;
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }
    }

    public void ResetMovement()
    {
        if (currentTarget != null)
        {
            Vector2 direction = ((Vector2)currentTarget.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }

        canMove = true;
        isKnockedBack = false;
    }

    public void ApplyKnockback(Vector2 direction, float force, float duration, bool isBoss = false)
    {
        if (!isKnockedBack)
        {
            knockbackDirection = direction.normalized;

            knockbackSpeed = isBoss ? force * 0.5f : force;
            StartCoroutine(KnockbackMovement(duration));
        }
    }

    private IEnumerator KnockbackMovement(float duration)
    {
        isKnockedBack = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position += (Vector3)(knockbackDirection * knockbackSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isKnockedBack = false;
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
}