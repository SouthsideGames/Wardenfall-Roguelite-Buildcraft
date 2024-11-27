using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("ELEMENTS:")]
    private CharacterManager player;

    [Header("SETTINGS:")]
    public float moveSpeed;
    private bool canMove = true;

    public void StorePlayer(CharacterManager _player)
    {
        player = _player;
    }

    public void FollowPlayer()
    {
        if (!canMove || player == null) return;

        Vector2 direction = (player.transform.position - transform.position).normalized;
        Vector2 targetPosition = (Vector2)transform.position + direction * moveSpeed * Time.deltaTime;

        transform.position = targetPosition;
    }

    public void MoveAwayFromPlayer()
    {
        if (!canMove || player == null) return;

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

}
