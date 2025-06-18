using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class ChargeEnemy : Enemy
{
    [Header("CHARGE SPECIFICS:")]
    [SerializeField] private float chargeTime = 2f; 
    [SerializeField] private float growFactor = 1.5f;
    [SerializeField] private float chargeSpeed = 20f; 
    [SerializeField] private float chargeDistance = 10f;
    [SerializeField] private float cooldownTime = 3f;
    [SerializeField] private bool multiCharge = false;
    [SerializeField] private int numberOfCharges = 3;
    [SerializeField] private float interChargeDelay = 0.4f;

    [Header("EFFECTS:")]
    private bool attackPerformed = false; 
    private Vector3 originalScale; 
    private Vector2 chargeDirection;
    private bool isCharging = true; 

    private void Awake() => OnSpawnCompleted += SpawnCompletedActions;
    private void OnDestroy() => OnSpawnCompleted -= SpawnCompletedActions; 

    protected override void Start()
    {
        base.Start();
        originalScale = transform.localScale;
    }

    protected override void Update()
    {
        base.Update();

        if (!isCharging)
            StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        isCharging = true;
        int charges = multiCharge ? numberOfCharges : 1;

        for (int i = 0; i < charges; i++)
        {
            attackPerformed = false;

            yield return Grow();
            LocatePlayer();
            yield return DashTowardsPlayer();
            yield return Shrink();

            if (i < charges - 1)
                yield return new WaitForSeconds(interChargeDelay);
        }

        yield return new WaitForSeconds(cooldownTime);
        isCharging = false;
    }

    private IEnumerator Grow()
    {
        float elapsed = 0f;
        Vector3 squashScale = new Vector3(originalScale.x * 0.75f, originalScale.y * 1.25f, 1f);
        Vector3 stretchScale = originalScale * growFactor;

        LeanTween.scale(gameObject, squashScale, 0.15f).setEaseOutSine();
        yield return new WaitForSeconds(0.15f);

        Color baseColor = _spriteRenderer.color;
        LeanTween.value(gameObject, 1f, 0.5f, chargeTime)
            .setLoopPingPong()
            .setEaseInOutSine()
            .setOnUpdate((float val) =>
            {
                if (_spriteRenderer != null)
                    _spriteRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, val);
            });

        GameObject particles = Instantiate(Resources.Load<GameObject>("VFX/ChargeParticles"), transform.position, Quaternion.identity, transform);

        LeanTween.moveLocalX(gameObject, transform.localPosition.x + 0.1f, 0.05f).setLoopPingPong(6);

        while (elapsed < chargeTime)
        {
            transform.localScale = Vector3.Lerp(squashScale, stretchScale, elapsed / chargeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = stretchScale;
        LeanTween.cancel(gameObject);
    }

    private void LocatePlayer()
    {
        if (this == null || gameObject == null || character == null || character.transform == null) 
        {
            chargeDirection = Vector2.right;
            return;
        }

        chargeDirection = (character.transform.position - transform.position).normalized;
    }

    private IEnumerator DashTowardsPlayer()
    {
        Vector3 dashStretch = new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, 1f);
        LeanTween.scale(gameObject, dashStretch, 0.1f).setEaseOutSine();

        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail != null) trail.emitting = true;

        Instantiate(Resources.Load<GameObject>("VFX/DashBurst"), transform.position, Quaternion.identity);

        SpriteRenderer ghost = Instantiate(_spriteRenderer, transform.position, Quaternion.identity);
        ghost.color = new Color(ghost.color.r, ghost.color.g, ghost.color.b, 0.4f);
        Destroy(ghost.gameObject, 0.3f);

        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + chargeDirection * chargeDistance;
        float distanceTraveled = 0f;

        while (distanceTraveled < chargeDistance)
        {
            float step = chargeSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);
            distanceTraveled += step;
            TryAttack();
            yield return null;
        }

        if (trail != null) trail.emitting = false;
    }

    private IEnumerator Shrink()
    {
        float elapsed = 0f;
        Vector3 undershootScale = originalScale * 0.85f;

        while (elapsed < chargeTime / 2f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, undershootScale, elapsed / (chargeTime / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        Instantiate(Resources.Load<GameObject>("VFX/CooldownPuff"), transform.position, Quaternion.identity);

        elapsed = 0f;
        while (elapsed < chargeTime / 2f)
        {
            transform.localScale = Vector3.Lerp(undershootScale, originalScale, elapsed / (chargeTime / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;

        LeanTween.cancel(gameObject);
        _spriteRenderer.color = Color.white;
    }

    private void TryAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);
        if (distanceToPlayer <= playerDetectionRadius && !attackPerformed)
            Attack();
    }

    protected override void Attack()
    {
        base.Attack();
        attackPerformed = true; 
    }

    protected override void ChangeDirections()
    {
        if (playerTransform != null)
        {
            if (playerTransform.position.x > transform.position.x && !attackPerformed)
                _spriteRenderer.flipX = true;
            else
                _spriteRenderer.flipX = false;
        }
    }

    private void SpawnCompletedActions() => isCharging = false;

    private void OnDrawGizmosSelected()
    {
        if (character != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)chargeDirection * chargeDistance);
        }
    }
}
