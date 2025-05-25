using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Add visual effect for the tethered enemies
public class HealerEnemy : Enemy
{
    [Header("HEALER SPECIFICS:")]
    [SerializeField] private float tetherRadius = 5f; // 
    [SerializeField] private int maxTetheredEnemies = 4; // 
    [SerializeField] private LayerMask enemyLayer; 
    [SerializeField] private GameObject healingEmoji;
    private List<Enemy> tetheredEnemies = new List<Enemy>(); 

    protected override void Start()
    {
        base.Start();
        StartCoroutine(ShowRandomEmotes());
    }

    protected override void SpawnCompleted()
    {
        base.SpawnCompleted();

        TetherRandomEnemies();
    }
    

    private void TetherRandomEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, tetherRadius, enemyLayer);
        List<Enemy> potentialEnemies = new List<Enemy>();

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemy) && enemy != this && !enemy.isInvincible)
            {
                potentialEnemies.Add(enemy);
            }
        }

        if (potentialEnemies.Count == 0)
        {
            Debug.Log("No eligible enemies found within tether radius.");
            return;
        }

        while (tetheredEnemies.Count < maxTetheredEnemies && potentialEnemies.Count > 0)
        {
            int randomIndex = Random.Range(0, potentialEnemies.Count);
            Enemy selectedEnemy = potentialEnemies[randomIndex];
            tetheredEnemies.Add(selectedEnemy);
            selectedEnemy.isInvincible = true; 
            potentialEnemies.RemoveAt(randomIndex);
        }

        Debug.Log($"Totem tethered {tetheredEnemies.Count} enemies, making them invincible.");
    }

    public override void Die()
    {
        base.Die(); 
        UntetherEnemies();
    }

    private IEnumerator ShowRandomEmotes()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 8f));
            
            if (healingEmoji != null)
            {
                var emote = Instantiate(healingEmoji, transform.position + Vector3.up, Quaternion.identity);
                emote.transform.SetParent(transform);
                Destroy(emote, 1f);
            }
        }
    }

    public override void OnHit()
    {
        base.OnHit();
        // Run away dramatically when hit
        if (movement != null)
        {
            Vector2 awayFromPlayer = (transform.position - playerTransform.position).normalized;
            movement.AddForce(awayFromPlayer * 5f);
        }
    }


    private void UntetherEnemies()
    {
        foreach (Enemy enemy in tetheredEnemies)
        {
            if (enemy != null)
            {
                enemy.isInvincible = false;
            }
        }

        tetheredEnemies.Clear();

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, tetherRadius);
    }
}
