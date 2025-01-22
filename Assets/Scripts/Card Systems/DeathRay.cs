using System.Collections;
using UnityEngine;

public class DeathRay : MonoBehaviour
{
   [Tooltip("Time between random beam spawns.")]
    [SerializeField] private float beamSpawnInterval = 1f;

    [Tooltip("Beam prefab.")]
    [SerializeField] private GameObject deathBeamPrefab;

    [Tooltip("LayerMask to detect enemies.")]
    [SerializeField] private LayerMask enemyMask;

    private float duration;

    public void Configure(float duration)
    {
        this.duration = duration;
    }

    private void Start()
    {
        Destroy(gameObject, duration); // Destroy the cloud after its duration
        StartCoroutine(SpawnDeathBeams());
    }

    private IEnumerator SpawnDeathBeams()
    {
        while (duration > 0)
        {
            SpawnRandomBeam();
            yield return new WaitForSeconds(beamSpawnInterval);
        }
    }

    private void SpawnRandomBeam()
    {
        // Detect all enemies within range
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f, enemyMask);

        if (enemies.Length > 0)
        {
            // Select a random enemy from the detected ones
            Collider2D target = enemies[Random.Range(0, enemies.Length)];
            Vector2 targetPosition = target.transform.position;

            // Spawn the death beam at the target's position
            GameObject beam = Instantiate(deathBeamPrefab, targetPosition, Quaternion.identity);

            // Configure the beam to target the selected enemy
            DeathBeam beamScript = beam.GetComponent<DeathBeam>();
            if (beamScript != null)
            {
                beamScript.Configure(target.gameObject);
            }

            Debug.Log($"Death beam spawned at {targetPosition} targeting {target.name}!");
        }
        else
        {
            Debug.Log("No enemies found to target with the death beam.");
        }
    }
}
