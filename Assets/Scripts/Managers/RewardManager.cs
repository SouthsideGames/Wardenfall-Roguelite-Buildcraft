using UnityEngine;
using System;
using System.Collections.Generic;

public class RewardManager : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Transform rewardSpawnArea;
    [SerializeField] private GameObject cashPrefab;
    [SerializeField] private GameObject gemPrefab;
    [SerializeField] private GameObject cardPrefab;

    [Header("SETTINGS:")]
    [SerializeField] WaveManager waveManager;
    [SerializeField] private float baseRewardCount = 3;
    [SerializeField] private float rewardScalingFactor = 0.5f;

    [Header("REWARD PROBABILITY")]
    [Range(0f, 1f)] public float cashProbability = 0.5f;
    [Range(0f, 1f)] public float gemProbability = 0.3f;
    [Range(0f, 1f)] public float cardProbability = 0.2f;

    [Header("SCALING:")]
    [SerializeField] private int baseCashReward = 50;
    [SerializeField] private int cashMultiplier = 10;
    [SerializeField] private int baseGemReward = 10;
    [SerializeField] private int gemMultiplier = 2;

    private const int MAX_REWARDS = 5;
    private List<GameObject> rewards = new List<GameObject>();
    private bool rewardsGenerated = false;

    private void Awake() => WaveManager.OnSurvivalCompleted += ShowRewards;

    private void OnDestroy() => WaveManager.OnSurvivalCompleted -= ShowRewards;

    public void ShowRewards()
    {
        if (!rewardsGenerated) 
        {
            GenerateRewards(waveManager.SurvivalTime);
            rewardsGenerated = true; 
        }
    }

    private void GenerateRewards(float survivalTime)
    {
        ClearPreviousRewards();

        int rewardCount = Mathf.CeilToInt(baseRewardCount + (survivalTime * rewardScalingFactor));
        rewardCount = Mathf.Min(rewardCount, MAX_REWARDS);

        Debug.Log($"Generating {rewardCount} rewards...");

        for (int i = 0; i < rewardCount; i++)
        {
            float randomValue = UnityEngine.Random.value;
            Debug.Log($"Random Value: {randomValue}");

            if (randomValue < cardProbability)
            {
                Debug.Log("Spawning Card Reward");
                SpawnReward(cardPrefab, 0, ItemRewardType.Card);
            }
            else if (randomValue < cardProbability + gemProbability)
            {
                int gemAmount = baseGemReward + Mathf.FloorToInt(survivalTime * gemMultiplier);
                Debug.Log($"Spawning Gem Reward: {gemAmount}");
                SpawnReward(gemPrefab, gemAmount, ItemRewardType.Gem);
            }
            else
            {
                int cashAmount = baseCashReward + Mathf.FloorToInt(survivalTime * cashMultiplier);
                Debug.Log($"Spawning Cash Reward: {cashAmount}");
                SpawnReward(cashPrefab, cashAmount, ItemRewardType.Cash);
            }
        }
    }

    private void SpawnReward(GameObject prefab, int amount, ItemRewardType rewardType)
    {
        GameObject reward = Instantiate(prefab, rewardSpawnArea);
        Reward rewardItem = reward.GetComponent<Reward>();
        if (rewardItem != null)
        {
            rewardItem.SetAmount(amount, rewardType);
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
            // Ensure the reward is processed properly
            if (reward.TryGetComponent<Reward>(out Reward rewardItem))
            {
                rewardItem.Collect(); // Trigger reward-specific collection logic
            }
        }

        // Clear rewards after processing
        ClearPreviousRewards();

        // Move to the menu after all rewards are collected
        GameManager.Instance.SetGameState(GameState.Menu);
    }
}
