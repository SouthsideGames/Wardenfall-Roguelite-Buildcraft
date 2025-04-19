using System.Collections;
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
        if (!canMove || currentTarget == null || isKnockedBack || isTargetPositionSet) return;

        Vector2 direction = ((Vector2)currentTarget.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
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


    

    
}
