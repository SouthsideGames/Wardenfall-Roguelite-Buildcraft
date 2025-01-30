using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("ELEMENTS:")]
    private Transform currentTarget;

    [Header("SETTINGS:")]
    public float moveSpeed;
    private bool canMove = true;
    private bool isKnockedBack = false;
    
    private Vector2 knockbackDirection;
    private float knockbackSpeed = 0f;

    public void StorePlayer(CharacterManager _player)
    {
        currentTarget = _player.transform;
    }

    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }

    public void FollowCurrentTarget()
    {
        if (!canMove || currentTarget == null || isKnockedBack) return;

        Vector2 direction = ((Vector2)currentTarget.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    public void MoveAwayFromCurrentTarget()
    {
        if (!canMove || currentTarget == null || isKnockedBack) return;

        Vector2 direction = ((Vector2)transform.position - (Vector2)currentTarget.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    public void DisableMovement(float duration)
    {
        StartCoroutine(DisableMovementTemporarily(duration));
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
        canMove = true;
        isKnockedBack = false;
    }
}
