using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyEvolutionHandler : MonoBehaviour
{
    [SerializeField] private bool isEvolvable = false;
    private bool hasEvolved = false;
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public bool CanEvolve() => isEvolvable && !hasEvolved && enemy.IsAlive;

    public void Evolve()
    {
        if (!CanEvolve()) return;
        hasEvolved = true;

        var evoData = enemy.enemyData.EvolutionData;
        if (evoData == null || evoData.EvolutionPrefab == null)
        {
            Debug.LogWarning("Evolve failed: Missing EvolutionData or EvolutionPrefab.");
            return;
        }

        Vector3 spawnPosition = transform.position;
        Quaternion rotation = transform.rotation;

        // Destroy current enemy
        Destroy(enemy.gameObject);

        // Instantiate evolved enemy
        GameObject evolved = Instantiate(evoData.EvolutionPrefab, spawnPosition, rotation);
        Enemy evolvedEnemy = evolved.GetComponent<Enemy>();
        
        if (evolvedEnemy != null && evolvedEnemy.enemyData != null)
        {
            Debug.Log($"[Evolve] Spawned evolved enemy: {evolvedEnemy.enemyData.ID} - {evolvedEnemy.enemyData.Name}");
        }
        else
        {
            Debug.LogWarning("Evolved enemy prefab missing Enemy or EnemyDataSO.");
        }
    }

}