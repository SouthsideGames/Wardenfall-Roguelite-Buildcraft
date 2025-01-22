using System.Collections;
using UnityEngine;

public class NecromancerCallEffect : ICardEffect
{
      private GameObject undeadMinionPrefab;
    private Transform spawnPoint;
    private float activeTime;
    private int minionCount = 5;
    private float spawnInterval = 2f; 
    private CardSO cardSO;

    public NecromancerCallEffect(GameObject _undeadMinionPrefab, Transform _spawnPoint, CardSO _card)
    {
        undeadMinionPrefab = _undeadMinionPrefab;
        spawnPoint = _spawnPoint;
        cardSO = _card;
    }

    public void Activate(float duration)
    {
        activeTime = duration;
        CoroutineRunner.Instance.StartCoroutine(SpawnUndeadMinions());
    }

    public void Disable()
    {
        Debug.Log("Necromancer’s Call effect disabled.");
    }

    private IEnumerator SpawnUndeadMinions()
    {
        float timer = 0f;
        int spawnedCount = 0;

        while (timer < activeTime && spawnedCount < minionCount)
        {
            GameObject minion = Object.Instantiate(undeadMinionPrefab, spawnPoint.position, Quaternion.identity);

            // Initialize minion with explosion damage
            UndeadMinion minionManager = minion.GetComponent<UndeadMinion>();
            minionManager.InitializeMinion(activeTime - timer, cardSO);

            spawnedCount++;
            timer += spawnInterval;

            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log($"Spawned {spawnedCount} undead minions.");
    }
}
