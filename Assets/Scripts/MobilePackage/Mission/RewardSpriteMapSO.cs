using UnityEngine;

namespace SouthsideGames.DailyMissions
{
    [CreateAssetMenu(fileName = "Reward Sprite Map ", menuName = "Scriptable Objects/Daily Missions/New Reward Sprite Map", order = 2)]
    public class RewardSpriteMapSO : ScriptableObject
    {
        [Header("DATA:")]
        [SerializeField] private RewardSpriteData[] data;
        public RewardSpriteData[] Data => data; 

        [SerializeField] private Sprite defaultSprite;

        public Sprite GetSprite(RewardType rewardType)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if(data[i].type == rewardType)
                    return data[i].sprite;
                
            }

            Debug.LogWarning("No sprite found for this reward type : " + rewardType);
            return defaultSprite;
        }
        
    }

    [System.Serializable]
    public struct RewardSpriteData
    {
        public RewardType type;
        public Sprite sprite;
    }
}
