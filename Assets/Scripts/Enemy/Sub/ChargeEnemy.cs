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
    [SerializeField] private GameObject ChargeParticles;
    [SerializeField] private GameObject CooldownPuff;
    [SerializeField] private float knockbackForce = 6f;

    private Vector3 originalScale;
    private Vector2 chargeDirection;
    private bool isCharging = true;
    private bool hasHitPlayer = false;

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
        hasHitPlayer = false; // reset hit state per charge cycle
        int charges = multiCharge ? numberOfCharges : 1;

        for (int i = 0; i < charges; i++)
        {
            EnableAttacks();
            isInvincible = true;

            yield return Grow();
            LocatePlayer();
            yield return DashTowardsPlayer();
            yield return Shrink();

            isInvincible = false;
            ChangeDirections();

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

        Instantiate(ChargeParticles, transform.position, Quaternion.identity, transform);
        LeanTween.moveLocalX(gameObject, transform.localPosition.x + 0.1f, 0.05f).setLoopPingPong(6);

        Color baseColor = _spriteRenderer.color;
        float flashTime = 0.2f;
        int flashCount = Mathf.FloorToInt(chargeTime / (flashTime * 2));

        for (int i = 0; i < flashCount; i++)
        {
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashTime);
            _spriteRenderer.color = baseColor;
            yield return new WaitForSeconds(flashTime);
        }

        elapsed = 0f;
        while (elapsed < chargeTime)
        {
            transform.localScale = Vector3.Lerp(squashScale, stretchScale, elapsed / chargeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = stretchScale;
        LeanTween.cancel(gameObject);
        _spriteRenderer.color = baseColor;
    }

    private void LocatePlayer()
    {
        if (character != null && character.transform != null)
            chargeDirection = (character.transform.position - transform.position).normalized;
        else
            chargeDirection = Vector2.right;
    }

    private IEnumerator DashTowardsPlayer()
    {
        Vector3 dashStretch = new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, 1f);
        LeanTween.scale(gameObject, dashStretch, 0.1f).setEaseOutSine();

        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail != null) trail.emitting = true;

        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + chargeDirection * chargeDistance;
        float dashDuration = chargeDistance / chargeSpeed;
        float elapsed = 0f;

        LeanTween.move(gameObject, targetPosition, dashDuration).setEaseOutSine();

        hasHitPlayer = false;
        float detectionRange = 0.4f;

        while (elapsed < dashDuration)
        {
            if (!hasHitPlayer && Vector2.Distance(transform.position, character.transform.position) <= detectionRange)
            {
                hasHitPlayer = true;
                Attack();

                Vector2 dir = (character.transform.position - transform.position).normalized;
                Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        DisableAttacks();

        if (trail != null) trail.emitting = false;
    }


    private IEnumerator Shrink()
    {
        float halfTime = chargeTime / 2f;
        float elapsed = 0f;
        Vector3 undershootScale = originalScale * 0.85f;

        while (elapsed < halfTime)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, undershootScale, elapsed / halfTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Instantiate(CooldownPuff, transform.position, Quaternion.identity);

        elapsed = 0f;
        while (elapsed < halfTime)
        {
            transform.localScale = Vector3.Lerp(undershootScale, originalScale, elapsed / halfTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        LeanTween.cancel(gameObject);
        _spriteRenderer.color = Color.white;
    }

    protected override void Attack()
    {
        base.Attack();
        DisableAttacks();
    }

    private void SpawnCompletedActions() => isCharging = false;

    protected override void ChangeDirections()
    {
        if (attacksEnabled && PlayerTransform != null)
            _spriteRenderer.flipX = PlayerTransform.position.x > transform.position.x;
    }

    private void OnDrawGizmosSelected()
    {
        if (character != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)chargeDirection * chargeDistance);
        }
    }
}
