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
    public float moveSpeed = 3f;
    public bool canMove = true;
    private bool isKnockedBack = false;

    private Vector2 knockbackDirection;
    private float knockbackSpeed = 0f;
    private float pathUpdateRate = 0.2f;
    private float avoidanceRadius = 1f;
    private float pathUpdateTimer;
    private Vector2 currentDirection;
    private Rigidbody2D rb;

    [Header("MOVEMENT PATTERNS:")]
    [Tooltip("Toggles that control how this enemy moves (e.g., chase, patrol, strafe). Each one enables a different behavior handled in FixedUpdate or helper methods.")]
    public bool chasePlayer = true;
    public bool wander = false;
    public bool patrol = false;
    public bool multiCharge = false;
    public bool advanceThenStop = false;
    public bool strafeAroundPlayer = false;
    public bool supportAnchor = false;
    public bool jitterChase = false;
    public bool teleportMovement = false;

    [Space(10)]
    [Header("PATROL SETTINGS")]
    [Tooltip("Settings for enemies that follow a loop of patrol points when 'patrol' is enabled.")]
    [SerializeField] private List<Transform> patrolPoints = new();
    private int currentPatrolIndex = 0;
    private float patrolTimer = 0f;
    [SerializeField] private float patrolWaitTime = 1f;
    [SerializeField] private float patrolPointReachedDistance = 0.1f;

    [Space(10)]
    [Header("WANDERING SETTINGS")]
    [Tooltip("Settings for enemies that roam randomly until detecting the player.")]
    [SerializeField] private float wanderRadius = 5f;
    private Vector2 wanderPoint;
    private float wanderTimer = 0f;
    [SerializeField] private float minWanderWaitTime = 2f;
    [SerializeField] private float maxWanderWaitTime = 5f;
    [SerializeField] private float wanderPointReachedDistance = 0.1f;
    [SerializeField] private float maxDistanceFromStart = 10f;

    [Space(10)]
    [Header("ADVANCE-THEN-STOP SETTINGS")]
    [Tooltip("Settings for enemies that stop after getting close enough to the player.")]
    [SerializeField] private float stopDistance = 6f;

    [Space(10)]
    [Header("STRAFE SETTINGS")]
    [Tooltip("Settings for enemies that orbit around the player at a set distance.")]
    [SerializeField] private float strafeRadius = 6f;
    [SerializeField] private float strafeOrbitSpeed = 120f;
    private float strafeAngleOffset = 0f;

    [Space(10)]
    [Header("JITTER CHASE SETTINGS")]
    [Tooltip("Settings for enemies that move toward the player with random direction jitter for unpredictability.")]
    [SerializeField] private float jitterFrequency = 0.3f;
    [SerializeField] private float jitterStrength = 1.2f;
    private float jitterTimer = 0f;

    [Space(10)]
    [Header("TELEPORT SETTINGS")]
    [Tooltip("Settings for enemies that teleport to new positions rather than moving directly.")]
    [SerializeField] private float teleportCooldown = 4.5f;
    [SerializeField] private float teleportDelay = 0.4f;
    [SerializeField] private float teleportDistanceMin = 3.5f;
    [SerializeField] private float teleportDistanceMax = 7f;
    [SerializeField] private GameObject teleportVFX;
    [SerializeField] private AudioClip teleportSFX;
    private bool teleportTriggeredThisFrame = false;
    private float teleportTimer = 0f;
    private bool isTeleporting = false;

    [Space(10)]
    [Header("ADDITIONAL SETTINGS")]
    [Tooltip("Settings that apply to all movement modes, such as obstacle avoidance or knockback.")]
    [SerializeField] private LayerMask obstacleLayer;
    private Vector2 externalForce;
    [SerializeField] private float detectionRange = 5f;

    private Vector2 startPosition;

    private bool hasAnticipated = false;


    private void Awake()
    {
        LeanTween.init(1200);
    }

   private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (CharacterManager.Instance != null)
            currentTarget = CharacterManager.Instance.transform;

        startPosition = transform.position;
        teleportTimer = 0f;
    }

    private void OnDestroy()
    {
        LeanTween.cancel(gameObject);
    }

    private void FixedUpdate()
    {
        teleportTriggeredThisFrame = false;
        if (!canMove || isKnockedBack || currentTarget == null) return;

        // Strafe-around logic
        if (strafeAroundPlayer)
        {
            HandleStrafeAroundPlayer();
            return;
        }

        if (teleportMovement)
        {
            HandleTeleportMovement();
            return;
        }

        if (jitterChase)
        {
            HandleJitterChase();
            return;
        }

        if (supportAnchor)
        {
            HandleSupportAnchor();
            return;
        }

        if (wander && !chasePlayer)
        {
            float distance = Vector2.Distance(transform.position, currentTarget.position);
            if (distance < detectionRange)
            {
                chasePlayer = true;
                wander = false;
                moveSpeed += 1.0f;
            }
        }

        if (advanceThenStop)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, currentTarget.position);
            if (distanceToPlayer > stopDistance)
            {
                Vector2 direction = ((Vector2)currentTarget.position - (Vector2)transform.position).normalized;
                Vector2 newPos = (Vector2)transform.position + direction * moveSpeed * Time.fixedDeltaTime + externalForce * Time.fixedDeltaTime;
                rb.MovePosition(newPos);
            }
            externalForce = Vector2.Lerp(externalForce, Vector2.zero, Time.deltaTime * 5f);
            return;
        }

        if (chasePlayer)
        {
            Vector2 direction = ((Vector2)currentTarget.position - (Vector2)transform.position).normalized;
            Vector2 newPos = (Vector2)transform.position + direction * moveSpeed * Time.fixedDeltaTime + externalForce * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
            externalForce = Vector2.Lerp(externalForce, Vector2.zero, Time.deltaTime * 5f);
        }

        if (patrol)
        {
            HandlePatrol();
            return;
        }
    }

    public void StorePlayer(CharacterManager _player) => currentTarget = _player.transform;

    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
        isTargetPositionSet = false; 
    }

    public void SetTargetPosition(Vector2 newPosition)
    {
        targetPosition = newPosition;
        isTargetPositionSet = true; 
    }

    private void PlayAnticipationAnimation()
    {
        if (TryGetComponent<EnemyAnimator>(out var animator))
        {
            animator.PlayGroggyMove(); // or PlayIdlePulse() or your custom one
        }
    }

   public void FollowCurrentTarget()
    {
        if (!canMove || rb == null) return;

        if (chasePlayer && currentTarget != null)
        {
            ChaseTarget();
        }
        else if (wander)
        {
            HandleWandering();
        }
        else if (patrol && patrolPoints != null && patrolPoints.Count > 0)
        {
            HandlePatrol();
        }
    }


  private void HandlePatrol()
    {
        if (currentTarget == null || patrolPoints == null || patrolPoints.Count == 0)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, currentTarget.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Chase player
            Vector2 direction = ((Vector2)currentTarget.position - (Vector2)transform.position).normalized;
            Vector2 newPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
            return;
        }

        // Otherwise, patrol
        if (patrolTimer > 0)
        {
            patrolTimer -= Time.deltaTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        if (targetPoint == null) return;

        Vector2 dirToPoint = (targetPoint.position - transform.position).normalized;
        rb.linearVelocity = dirToPoint * moveSpeed + externalForce;
        externalForce = Vector2.Lerp(externalForce, Vector2.zero, Time.deltaTime * 5f);

        if (Vector2.Distance(transform.position, targetPoint.position) < patrolPointReachedDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            patrolTimer = patrolWaitTime;
        }
    }

    private void HandleSupportAnchor()
    {
        Enemy woundedAlly = GetComponent<Enemy>().FindClosestWoundedAlly(10f);
        if (woundedAlly != null)
        {
            Vector2 targetPos = woundedAlly.transform.position;
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleJitterChase()
    {
        jitterTimer -= Time.fixedDeltaTime;

        Vector2 baseDirection = ((Vector2)currentTarget.position - (Vector2)transform.position).normalized;

        // Apply jitter periodically
        if (jitterTimer <= 0f)
        {
            float randomAngle = Random.Range(-jitterStrength, jitterStrength);
            baseDirection = Quaternion.Euler(0, 0, randomAngle) * baseDirection;
            jitterTimer = jitterFrequency;
        }

        Vector2 newPos = (Vector2)transform.position + baseDirection.normalized * moveSpeed * Time.fixedDeltaTime + externalForce * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
        externalForce = Vector2.Lerp(externalForce, Vector2.zero, Time.deltaTime * 5f);
    }

    private void HandleTeleportMovement()
    {
        teleportTimer -= Time.fixedDeltaTime;
        if (teleportTimer > 0 || isTeleporting) return;

        StartCoroutine(TeleportRoutine());
        teleportTimer = teleportCooldown;
    }

    public bool TeleportJustHappened()
    {
        return teleportTriggeredThisFrame;
    }

    private IEnumerator TeleportRoutine()
    {
        isTeleporting = true;

        if (teleportVFX != null)
            teleportVFX.SetActive(true); 

        if (teleportSFX != null)
            AudioManager.Instance.PlaySFX(teleportSFX);

        yield return new WaitForSeconds(teleportDelay);

        Vector2 playerPos = currentTarget.position;
        Vector2 teleportOffset;

        for (int i = 0; i < 20; i++) 
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(teleportDistanceMin, teleportDistanceMax);
            teleportOffset = Quaternion.Euler(0, 0, angle) * Vector2.right * distance;

            Vector2 targetPos = playerPos + teleportOffset;

            if (!Physics2D.OverlapCircle(targetPos, 0.5f, obstacleLayer))
            {
                transform.position = targetPos;
                teleportTriggeredThisFrame = true;
                break;
            }
        }

        if (teleportVFX != null)
            teleportVFX.SetActive(false);

        isTeleporting = false;
    }

    public void FollowCurrentTargetUntilRange(float stopDistance)
    {
        if (!canMove || rb == null || currentTarget == null) return;

        float distance = Vector2.Distance(transform.position, currentTarget.position);
        if (distance > stopDistance)
        {
            Vector2 direction = (currentTarget.position - transform.position).normalized;
            Vector2 newPos = (Vector2)transform.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }
        else
        {
            // Play anticipation animation once when in range
            if (!hasAnticipated)
            {
                hasAnticipated = true;
                PlayAnticipationAnimation();
            }
        }
    }




    private void ChaseTarget()
    {
        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateRate)
        {
            UpdatePath();
            pathUpdateTimer = 0;
        }

        Vector2 avoidanceForce = CalculateAvoidanceForce();
        Vector2 finalDirection = (currentDirection + avoidanceForce + externalForce).normalized;
        rb.linearVelocity = finalDirection * moveSpeed;

        //Decay external force
        externalForce = Vector2.Lerp(externalForce, Vector2.zero, Time.deltaTime * 5f);
    }

    private void HandleWandering()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0 || Vector2.Distance(transform.position, wanderPoint) < wanderPointReachedDistance)
        {
            GenerateNewWanderPoint();
            wanderTimer = Random.Range(minWanderWaitTime, maxWanderWaitTime);
        }

        // Check if too far from start position
        if (Vector2.Distance(transform.position, startPosition) > maxDistanceFromStart)
        {
            wanderPoint = startPosition;
        }

        Vector2 direction = (wanderPoint - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed + externalForce;
        externalForce = Vector2.Lerp(externalForce, Vector2.zero, Time.deltaTime * 5f);
    }

    private void GenerateNewWanderPoint()
    {
        for (int i = 0; i < 30; i++) 
        {
            float randomAngle = Random.Range(0f, 360f);
            Vector2 potentialPoint = (Vector2)transform.position + (Vector2)(Quaternion.Euler(0, 0, randomAngle) * Vector2.right * wanderRadius);

            if (!Physics2D.OverlapCircle(potentialPoint, 0.5f, obstacleLayer))
            {
                wanderPoint = potentialPoint;
                return;
            }
        }

        wanderPoint = startPosition;
    }

    private void HandleStrafeAroundPlayer()
    {
        if (currentTarget == null) return;

        Vector2 playerPos = currentTarget.position;
        strafeAngleOffset += strafeOrbitSpeed * Time.fixedDeltaTime;
        if (strafeAngleOffset > 360f) strafeAngleOffset -= 360f;

        float angleRad = strafeAngleOffset * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * strafeRadius;
        Vector2 targetPosition = (Vector2)playerPos + offset;

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        Vector2 newPos = (Vector2)transform.position + direction * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPos);
    }

    private void UpdatePath()
    {
        if (currentTarget == null) return;

        Vector2 targetPos = currentTarget.position;
        Vector2 directionToTarget = (targetPos - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, 5f);
        if (hit.collider != null && !hit.collider.CompareTag("Player"))
        {
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
        List<Enemy> nearbyEnemies = new List<Enemy>(); 

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<Enemy>(out var enemy) && enemy != this)
            {
                nearbyEnemies.Add(enemy);
            }
        }

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
            isTargetPositionSet = false; 
        }
    }

    public void DisableMovement(float duration) => DisableMovementTemporarily(duration);

    public void EnableMovement() => canMove = true;

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

    public void AddForce(Vector2 force)
    {
        externalForce += force;
    }

    public void StopMoving()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }


}

public class HazardAwareness : MonoBehaviour
{
    public float hazardAvoidanceForce = 10f;
    public LayerMask hazardLayer;

    private EnemyMovement enemyMovement;

    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void FixedUpdate()
    {
        Collider2D[] hazards = Physics2D.OverlapCircleAll(transform.position, 5f, hazardLayer);
        foreach (Collider2D hazard in hazards)
        {
            Vector2 directionToHazard = (Vector2)(transform.position - hazard.transform.position).normalized;
            enemyMovement.AddForce(directionToHazard * hazardAvoidanceForce);
        }
    }
}

