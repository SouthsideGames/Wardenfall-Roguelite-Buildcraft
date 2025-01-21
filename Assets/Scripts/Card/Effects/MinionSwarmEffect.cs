using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSwarmEffect : ICardEffect
{
    private GameObject minionPrefab;
    private Transform spawnPoint;
    private float activeTime;
    private int minionDamage;
    private int minionCount;
    private float spawnInterval;

    public MinionSwarmEffect(GameObject minionPrefab, Transform spawnPoint, int minionDamage, int minionCount, float spawnInterval)
    {
        this.minionPrefab = minionPrefab;
        this.spawnPoint = spawnPoint;
        this.minionDamage = minionDamage;
        this.minionCount = minionCount;
        this.spawnInterval = spawnInterval;
    }

    public void Activate(float duration)
    {
        activeTime = duration;
        CoroutineRunner.Instance.StartCoroutine(SpawnMinions());
    }

    public void Disable()
    {
        Debug.Log("Minion Swarm effect disabled.");
    }

    private IEnumerator SpawnMinions()
    {
        float timer = 0f;
        int spawnedCount = 0;

        while (timer < activeTime && spawnedCount < minionCount)
        {
            GameObject minion = Object.Instantiate(minionPrefab, spawnPoint.position, Quaternion.identity);
            MinionManager minionManager = minion.GetComponent<MinionManager>();
            minionManager.InitializeMinion(activeTime - timer, minionDamage);

            spawnedCount++;
            timer += spawnInterval;

            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log($"Spawned {spawnedCount} minions.");
    }
}
