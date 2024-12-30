using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("ELEMENTS:")]
    private CharacterManager player;

    [Header("SETTINGS:")]
    public float moveSpeed;
    private bool canMove = true;

    private Vector2 knockbackDirection;
    private float knockbackSpeed = 0f;
    private bool isKnockedBack = false;

    public void StorePlayer(CharacterManager _player)
    {
        player = _player;
    }

    public void FollowPlayer()
    {
        if (!canMove || player == null || isKnockedBack) return;

        Vector2 direction = (player.transform.position - transform.position).normalized;
        Vector2 targetPosition = (Vector2)transform.position + direction * moveSpeed * Time.deltaTime;

        transform.position = targetPosition;
    }

    public void MoveAwayFromPlayer()
    {
        if (!canMove || player == null || isKnockedBack) return;

        Vector2 direction = (transform.position - player.transform.position).normalized;
        Vector2 targetPosition = (Vector2)transform.position + direction * moveSpeed * Time.deltaTime;

        transform.position = targetPosition;
    }

    public void DisableMovement(float duration)
    {
        StartCoroutine(DisableMovementTemporarily(duration));
    }

    private IEnumerator DisableMovementTemporarily(float duration)
    {
        canMove = false;
        yield return new WaitForSeconds(duration);
        canMove = true;
    }

    // **NEW: Knockback Movement**
    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        if (!isKnockedBack)
        {
            knockbackDirection = direction.normalized;
            knockbackSpeed = force;
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
