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

    public MinionSwarmEffect(GameObject _minionPrefab, Transform _spawnPoint, int _minionDamage, int _minionCount, float _spawnInterval)
    {
        minionPrefab = _minionPrefab;
        spawnPoint = _spawnPoint;
        minionDamage = _minionDamage;
        minionCount = _minionCount;
        spawnInterval = _spawnInterval;
    }

    public void Activate(float duration)
    {
        activeTime = duration;
        CoroutineRunner.Instance.StartCoroutine(SpawnMinions());
    }

    public void Disable()
    {
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

    }
}
