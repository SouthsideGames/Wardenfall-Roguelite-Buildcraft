using UnityEngine;
using System;

namespace SouthsideGames.DailyMissions
{
    [System.Serializable]
    public class Mission
    {
        public static Action<Mission> updateMission;
        public static Action<Mission> completeMission;

        private MissionDataSO data;
        public MissionDataSO Data => data;

        private bool isComplete;
        public bool IsComplete => isComplete;   

        private bool isClaimed;
        public bool IsClaimed => isClaimed;

        private int amount;
        public int Amount
        {
            get => amount;
            set
            {
                if(isComplete)
                   return;
                   
                amount = Mathf.Min(value, data.Target);

                updateMission?.Invoke(this);

                if(amount == data.Target)
                   CompleteMission();
            }
        }

        public float Progress => (float)amount / data.Target;   
        public string ProgressString => amount + "/" + data.Target;

        public MissionType Type => data.Type;

        public Mission(MissionDataSO _data)
        {
            this.data = _data;  
        }    

        public void CompleteMission()
        {
            isComplete = true;
            completeMission?.Invoke(this);  
        }

        public void Claim()
        {
            isComplete = true;
            isClaimed = true;
        }
    }
}

