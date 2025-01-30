using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEnemy : Enemy
{
    [Header("CHARGE SPECIFICS:")]
    [SerializeField] private float chargeTime = 2f; 
    [SerializeField] private float growFactor = 1.5f;
    [SerializeField] private float chargeSpeed = 20f; 
    [SerializeField] private float chargeDistance = 10f;
    [SerializeField] private float cooldownTime = 3f;

    [Header("EFFECTS:")]
    [SerializeField] private GameObject objectToSpawn;
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
        AnimationLogic();

        if (!isCharging)
          StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        isCharging = true; 
        attackPerformed = false; 
        yield return StartCoroutine(Grow());

        LocatePlayer();

        yield return StartCoroutine(DashTowardsPlayer());

        yield return StartCoroutine(Shrink());

        yield return new WaitForSeconds(cooldownTime);

        isCharging = false; 
    }

    private IEnumerator Grow()
    {     
        
        float elapsed = 0f;
        Vector3 targetScale = originalScale * growFactor;

        while (elapsed < chargeTime)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / chargeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private void LocatePlayer()
    {
        if (character != null)
            chargeDirection = (character.transform.position - transform.position).normalized; 
        else
            chargeDirection = Vector2.right;
    }

    private IEnumerator DashTowardsPlayer()
    {
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + chargeDirection * chargeDistance;

        float distanceTraveled = 0f;

        while (distanceTraveled < chargeDistance)
        {
            float step = chargeSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);
            distanceTraveled += step;

            TryAttack(); 

            SpawnEffect();

            yield return null;
        }
    }

    private IEnumerator Shrink()
    {
        float elapsed = 0f;
        Vector3 targetScale = originalScale;

        while (elapsed < chargeTime)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, elapsed / chargeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
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
            if (playerTransform.position.x > transform.position.x && !attackPerformed )
                _spriteRenderer.flipX = true;
            else
                _spriteRenderer.flipX = false;
        }
    }

    private void SpawnEffect() => Instantiate(objectToSpawn, transform.position, Quaternion.identity);
    private void AnimationLogic() => anim.SetBool("isIdle", !attackPerformed);
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
