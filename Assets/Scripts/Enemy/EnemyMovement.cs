using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Elements")]
    private PlayerManager player;

    [Header("Settings")]
    [SerializeField] private float moveSpeed;
    
    public void StorePlayer(PlayerManager _player)
    {
        this.player = _player;   
    }

    public void FollowPlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;

        Vector2 targetPosition = (Vector2)transform.position + direction * moveSpeed * Time.deltaTime;

        transform.position = targetPosition;
    }


}
