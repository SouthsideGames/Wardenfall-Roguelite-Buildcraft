using UnityEngine;
using System.Collections.Generic;

public class HazardManager : MonoBehaviour
{
    public static HazardManager Instance;

    [Header("Hazards")]
    [SerializeField] private GameObject[] hazardPrefabs;
    [SerializeField] private int minHazardsPerWave = 2;
    [SerializeField] private int maxHazardsPerWave = 5;
    [SerializeField] private float minSpawnDistance = 3f;
    [SerializeField] private float maxSpawnDistance = 8f;

    private List<GameObject> activeHazards = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnHazards(Vector3 centerPoint)
    {
        ClearHazards();
        
        int hazardCount = Random.Range(minHazardsPerWave, maxHazardsPerWave + 1);
        
        for (int i = 0; i < hazardCount; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector2 spawnOffset = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad) * distance,
                Mathf.Sin(angle * Mathf.Deg2Rad) * distance
            );

            Vector3 spawnPosition = centerPoint + (Vector3)spawnOffset;
            GameObject hazard = Instantiate(
                hazardPrefabs[Random.Range(0, hazardPrefabs.Length)],
                spawnPosition,
                Quaternion.identity
            );
            
            activeHazards.Add(hazard);
        }
    }

    public void ClearHazards()
    {
        foreach (GameObject hazard in activeHazards)
        {
            if (hazard != null)
                Destroy(hazard);
        }
        activeHazards.Clear();
    }
}
