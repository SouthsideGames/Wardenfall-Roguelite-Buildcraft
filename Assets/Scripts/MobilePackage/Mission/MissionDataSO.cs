using UnityEngine;

namespace SouthsideGames.DailyMissions
{
    [CreateAssetMenu(fileName = "Mission Data", menuName = "Scriptable Objects/Daily Missions/New Mission Data", order = 1)]
    public class MissionDataSO : ScriptableObject
    {
        [SerializeField] private MissionType type;
        public MissionType Type => type;

        [SerializeField] private int target;
        public int Target => target;    

        [SerializeField] private int rewardXP;
        public int RewardXP => rewardXP;

        [SerializeField] private Sprite icon;
        public Sprite Icon => icon; 
    }

}
