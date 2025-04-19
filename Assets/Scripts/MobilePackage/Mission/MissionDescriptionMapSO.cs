using UnityEngine;

namespace SouthsideGames.DailyMissions
{
    [CreateAssetMenu(fileName = "Mission Description Map ", menuName = "Scriptable Objects/Daily Missions/New Mission Description Map", order = 3)]
    public class MissionDescriptionMapSO : ScriptableObject
    {
        [Header("DATA:")]
        [SerializeField] private MissionDescriptionData[] data;
        public MissionDescriptionData[] Data => data; 

        public string GetDescription(MissionType missionType, int target)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if(data[i].type == missionType)
                    return string.Format(data[i].description, target);
                
            }

            Debug.LogWarning("No sprite found for this reward type : " + missionType);
            return "";
        }
        
    }

    [System.Serializable]
    public struct MissionDescriptionData
    {
        public MissionType type;
        public string description;
    }
}
