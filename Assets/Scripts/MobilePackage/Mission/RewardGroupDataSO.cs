using UnityEngine;
using SouthsideGames.DailyMissions;

namespace SouthsideGames.DailyMissions
{
    [CreateAssetMenu(fileName = "Reward Group Data", menuName = "Scriptable Objects/New Reward Group Data", order = 8)]
    public class RewardGroupDataSO : ScriptableObject
    {
        [SerializeField] private RewardMilestoneData[] rewardMilestoneDatas;
        public RewardMilestoneData[] RewardMilestoneDatas => rewardMilestoneDatas;
    }

    [System.Serializable]
    public struct RewardEntryData
    {
        public RewardType type;
        public int amount;
    }

    [System.Serializable]
    public struct RewardMilestoneData
    {
        public Sprite Icon;
        public RewardEntryData[] rewards;
        public int requiredXP;
    }

}  


