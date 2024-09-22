using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("ELEMENTS:")]
    private CharacterManager player;

    [Header("SETTINGS:")]
    [SerializeField] private float moveSpeed;
    
    public void StorePlayer(CharacterManager _player)
    {
        player = _player;   
    }

    public void FollowPlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;

        Vector2 targetPosition = (Vector2)transform.position + direction * moveSpeed * Time.deltaTime;

        transform.position = targetPosition;
    }


    public void MoveAwayFromPlayer()
    {
        // Calculate the direction away from the player
        Vector2 direction = (transform.position - player.transform.position).normalized;

        // Calculate the target position by moving away from the player
        Vector2 targetPosition = (Vector2)transform.position + direction * moveSpeed * Time.deltaTime;

        // Move the enemy to the target position
        transform.position = targetPosition;
    }


}
