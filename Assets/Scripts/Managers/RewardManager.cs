using UnityEngine;
using System;
using System.Collections.Generic;

public class RewardManager : MonoBehaviour
{
    [Header("Reward Panel Elements")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private Transform rewardSpawnArea;
    [SerializeField] private GameObject cashPrefab;
    [SerializeField] private GameObject gemPrefab;
    [SerializeField] private GameObject cardPrefab;

    [Header("Reward Settings")]
    [SerializeField] WaveManager waveManager;
    [SerializeField] private float baseRewardCount = 3;
    [SerializeField] private float rewardScalingFactor = 0.5f;

    [Header("Reward Probabilities")]
    [Range(0f, 1f)] public float cashProbability = 0.5f;
    [Range(0f, 1f)] public float gemProbability = 0.3f;
    [Range(0f, 1f)] public float cardProbability = 0.2f;

    [Header("Cash and Gem Scaling")]
    [SerializeField] private int baseCashReward = 50;
    [SerializeField] private int cashMultiplier = 10;
    [SerializeField] private int baseGemReward = 10;
    [SerializeField] private int gemMultiplier = 2;

    private List<GameObject> rewards = new List<GameObject>();

    private void Awake()
    {
        WaveManager.OnSurvivalCompleted += ShowRewards;
    }

    private void OnDestroy()
    {
        WaveManager.OnSurvivalCompleted -= ShowRewards;
    }

    public void ShowRewards()
    {
        rewardPanel.SetActive(true);
        GenerateRewards(waveManager.SurvivalTime);
    }

    private void GenerateRewards(float survivalTime)
    {
        ClearPreviousRewards();

        // Calculate reward count based on survival time
        int rewardCount = Mathf.CeilToInt(baseRewardCount + (survivalTime * rewardScalingFactor));

        for (int i = 0; i < rewardCount; i++)
        {
            float randomValue = UnityEngine.Random.value;

            if (randomValue < cardProbability)
            {
                SpawnReward(cardPrefab, 1); // Cards have no quantity displayed
            }
            else if (randomValue < cardProbability + gemProbability)
            {
                int gemAmount = baseGemReward + Mathf.FloorToInt(survivalTime * gemMultiplier);
                SpawnReward(gemPrefab, gemAmount);
            }
            else
            {
                int cashAmount = baseCashReward + Mathf.FloorToInt(survivalTime * cashMultiplier);
                SpawnReward(cashPrefab, cashAmount);
            }
        }
    }

    private void SpawnReward(GameObject prefab, int amount)
    {
        GameObject reward = Instantiate(prefab, rewardSpawnArea);
        Reward rewardItem = reward.GetComponent<Reward>();
        if (rewardItem != null)
        {
            rewardItem.SetAmount(amount);
        }
        rewards.Add(reward);
    }

    private void ClearPreviousRewards()
    {
        foreach (var reward in rewards)
        {
            Destroy(reward);
        }
        rewards.Clear();
    }

    public void CollectRewards()
    {
        foreach (var reward in rewards)
        {
            if (reward.TryGetComponent<Item>(out Item item))
            {
                item.Collect(CharacterManager.Instance);
            }
        }

        // Close reward panel and return to menu
        rewardPanel.SetActive(false);
        GameManager.Instance.SetGameState(GameState.Menu);
    }
}
