using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SouthsideGames.DailyMissions
{
    [RequireComponent(typeof(MissionManagerUI))]
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance;
        public static Action<int> xpUpdated;

        [Header("COMPONENTS:")]
        private MissionManagerUI uI;

        [Header("DATA")]
        [SerializeField] private MissionDataSO[] missionDatas;
        List<Mission> activeMissions = new List<Mission>(); 

        private int xp;
        public int Xp => xp;    

        private void Awake() 
        {
            if(Instance == null)
               Instance = this;
            else
                Destroy(gameObject);

            uI = GetComponent<MissionManagerUI>();  

            Mission.updateMission += OnMissionUpdated;
            Mission.completeMission += OnCompleteMission;
        }

        private void Start() 
        {
            for(int i = 0; i < missionDatas.Length; i++)
                activeMissions.Add(new Mission(missionDatas[i]));

            uI.Init(activeMissions.ToArray());
        }

        private void Destroy()
        {
            Mission.updateMission -= OnMissionUpdated;
            Mission.completeMission -= OnCompleteMission;
        }

        public void HandleMissionClaimed(int index)
        {
            Mission mission = activeMissions[index];
            mission.Claim();

            xp += mission.Data.RewardXp;
            xpUpdated?.Invoke(xp);
        }

        private void OnMissionUpdated(Mission _mission)
        {
            uI.UpdateMission(activeMissions.IndexOf(_mission));
        }
        
        private void OnCompleteMission(Mission _mission)
        {
 
        }

        public static void Increment(MissionType _missionType, int _amount)
        {
            for (int i = 0; i < Instance.activeMissions.Count; i++)
            {
                if(Instance.activeMissions[i].IsComplete || Instance.activeMissions[i].IsClaimed)
                    continue;
                    
                if(Instance.activeMissions[i].Type == _missionType)
                   Instance.activeMissions[i].Amount += _amount;
            }
        }


    }
}


