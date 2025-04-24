using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosDriftEffect : MonoBehaviour, ICardEffect
{
    private class MovementState
    {
        public bool chasePlayer;
        public bool wander;
        public bool patrol;
    }

    private Dictionary<EnemyMovement, MovementState> originalStates = new();
    private float duration;

    public void Activate(CharacterManager target, CardSO card)
    {
        duration = card.activeTime;

        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            ApplyWander(enemy);
        }

        StartCoroutine(RestoreAfterDelay());
        Destroy(gameObject, duration + 0.5f);
    }

    private void ApplyWander(Enemy enemy)
    {
        if (enemy == null) return;

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null && !originalStates.ContainsKey(movement))
        {
            originalStates[movement] = new MovementState
            {
                chasePlayer = movement.chasePlayer,
                wander = movement.wander,
                patrol = movement.patrol
            };

            movement.chasePlayer = false;
            movement.patrol = false;
            movement.wander = true;
        }
    }

    private IEnumerator RestoreAfterDelay()
    {
        yield return new WaitForSeconds(duration);

        foreach (var kvp in originalStates)
        {
            if (kvp.Key != null)
            {
                kvp.Key.chasePlayer = kvp.Value.chasePlayer;
                kvp.Key.wander = kvp.Value.wander;
                kvp.Key.patrol = kvp.Value.patrol;
            }
        }

        originalStates.Clear();
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
} 
