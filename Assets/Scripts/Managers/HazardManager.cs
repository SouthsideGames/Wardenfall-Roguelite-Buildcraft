using UnityEngine;
using System.Collections.Generic;

public class HazardManager : MonoBehaviour
{
    public static HazardManager Instance;

    [Header("Hazards")]
    [SerializeField] private GameObject[] hazardPrefabs;
    [SerializeField] private int minHazardsPerWave = 2;
    [SerializeField] private int maxHazardsPerWave = 5;
    [SerializeField] private int poolSizePerHazardType = 3;

    private List<GameObject> activeHazards = new List<GameObject>();
    private Dictionary<GameObject, List<GameObject>> hazardPools = new Dictionary<GameObject, List<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializePools();
    }

    public void SpawnHazards()
    {
        ClearHazards();

        int hazardCount = Random.Range(minHazardsPerWave, maxHazardsPerWave + 1);
        float halfWidth = Constants.arenaSize.x / 2f;
        float halfHeight = Constants.arenaSize.y / 2f;

        for (int i = 0; i < hazardCount; i++)
        {
            GameObject prefab = hazardPrefabs[Random.Range(0, hazardPrefabs.Length)];
            GameObject hazard = GetHazardFromPool(prefab);

            if (hazard != null)
            {
                float randomX = Random.Range(-halfWidth, halfWidth);
                float randomY = Random.Range(-halfHeight, halfHeight);
                Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

                hazard.transform.position = spawnPosition;
                hazard.transform.rotation = Quaternion.identity;
                hazard.SetActive(true);
                activeHazards.Add(hazard);
            }
        }
    }

    private void InitializePools()
    {
        foreach (GameObject prefab in hazardPrefabs)
        {
            List<GameObject> pool = new List<GameObject>();
            for (int i = 0; i < poolSizePerHazardType; i++)
            {
                GameObject obj = Instantiate(prefab, transform); // << Parent to HazardManager
                obj.SetActive(false);
                pool.Add(obj);
            }
            hazardPools.Add(prefab, pool);
        }
    }

    private GameObject GetHazardFromPool(GameObject prefab)
    {
        if (!hazardPools.ContainsKey(prefab))
            return null;

        foreach (GameObject hazard in hazardPools[prefab])
        {
            if (!hazard.activeInHierarchy)
            {
                return hazard;
            }
        }

        // OPTIONAL: Expand pool if needed
        GameObject newHazard = Instantiate(prefab, transform); // << Parent to HazardManager
        newHazard.SetActive(false);
        hazardPools[prefab].Add(newHazard);
        return newHazard;
    }

    public void ClearHazards()
    {
        foreach (GameObject hazard in activeHazards)
        {
            if (hazard != null)
                hazard.SetActive(false);
        }
        activeHazards.Clear();
    }
}
