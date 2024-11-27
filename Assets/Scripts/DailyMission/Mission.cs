using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Android.Gradle.Manifest;
using Unity.Mathematics;
using UnityEngine;

namespace SouthsideGames.DailyMissions
{
    [System.Serializable]
    public class Mission
    {
        private MissionDataSO data;
        public MissionDataSO Data => data;

        private bool isComplete;
        public bool IsComplete => isComplete;   

        private int amount;
        public int Amount
        {
            get => amount;
            set
            {
                if(isComplete)
                   return;
                   
                amount = Mathf.Min(value, data.Target);
                
                if(amount == data.Target)
                   CompleteMission();
            }
        }

        public MissionType Type => data.Type;

        public Mission(MissionDataSO _data)
        {
            this.data = _data;  
        }    

        public void CompleteMission()
        {
            Debug.Log("Mission Complete");
            isComplete = true;
        }
    }
}

