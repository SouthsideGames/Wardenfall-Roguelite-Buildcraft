using UnityEngine;

namespace SouthsideGames.DailyMissions
{
    public static class RewardSpriteMapper
    {
        static RewardSpriteMapSO data;

        static RewardSpriteMapper() => data = Resources.Load("Data/Mission Maps/ Reward Sprite Map") as RewardSpriteMapSO;
        public static Sprite GetSprite(RewardType type) => data.GetSprite(type);    
    }

}
