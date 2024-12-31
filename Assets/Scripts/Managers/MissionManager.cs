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

        [Header("COMPONENTS:")]
        private MissionManagerUI uI;

        [Header("DATA")]
        [SerializeField] private MissionDataSO[] missionDatas;
        List<Mission> activeMissions = new List<Mission>(); 

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

        public static void Increment(MissionType _missionType, int _amount)
        {
            for (int i = 0; i < Instance.activeMissions.Count; i++)
            {
                if(Instance.activeMissions[i].IsComplete)
                    continue;
                    
                if(Instance.activeMissions[i].Type == _missionType)
                   Instance.activeMissions[i].Amount += _amount;
            }
        }

        private void OnMissionUpdated(Mission _mission)
        {
            uI.UpdateMission(activeMissions.IndexOf(_mission));
        }
        
        private void OnCompleteMission(Mission _mission)
        {
 
        }

       

    }
}


