using UnityEngine;

namespace SouthsideGames.DailyMissions
{
    public static class MissionDescriptionMapper
    {
        static MissionDescriptionMapSO data;

        static MissionDescriptionMapper() => data = Resources.Load("Data/Mission Maps/Mission Description Map") as MissionDescriptionMapSO;
        public static string GetDescription(MissionDataSO missionData) => data.GetDescription(missionData.Type, missionData.Target);
    }
}

