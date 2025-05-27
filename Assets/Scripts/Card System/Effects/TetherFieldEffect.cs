using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherFieldEffect : MonoBehaviour, ICardEffect
{
    [Header("Tether Settings")]
    [SerializeField] private float tetherRadius = 6f;
    [SerializeField] private float tetherDuration = 6f;
    [SerializeField] private float damageSharePercent = 0.5f;
    [SerializeField] private LayerMask enemyMask;

    private List<EnemyTetheredHelper> tetheredEnemies = new();

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null)
        {
            Debug.LogWarning("TetherFieldEffect: Target is null.");
            return;
        }

        Vector2 center = target.transform.position;

        // Find all enemies in radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, tetherRadius, enemyMask);

        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out EnemyTetheredHelper tether) && !tether.enabled)
            {
                tether.enabled = true;
                tether.Initialize(damageSharePercent);
                tetheredEnemies.Add(tether);
            }
        }

        if (tetheredEnemies.Count > 1)
        {
            foreach (var tether in tetheredEnemies)
                tether.SetTetherGroup(tetheredEnemies);
        }

        StartCoroutine(TetherDurationTimer());
    }

    private IEnumerator TetherDurationTimer()
    {
        yield return new WaitForSeconds(tetherDuration);

        foreach (var tether in tetheredEnemies)
        {
            if (tether != null)
                tether.enabled = false;
        }

        tetheredEnemies.Clear();
        Destroy(gameObject); // Clean up the effect object
    }

    public void Deactivate() { }

    public void Tick(float deltaTime) { }
}
