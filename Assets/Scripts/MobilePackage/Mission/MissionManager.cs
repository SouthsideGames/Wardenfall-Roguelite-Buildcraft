using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIExtensions;

namespace SouthsideGames.DailyMissions
{
    [RequireComponent(typeof(MissionManagerUI))]
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance;
        public static Action<int> xpUpdated;

        [Header("COMPONENTS:")]
        private MissionManagerUI uI;

        [Header("DATA:")]
        [SerializeField] private MissionDataSO[] missionDatas;
        List<Mission> activeMissions = new List<Mission>(); 

        [Header("EFFECT:")]
        [SerializeField] private ParticleSystem missionCollectParticles;
        [SerializeField] private Transform particleParent;
        private UIParticleAttractor uIParticleAttractor;


        private int xp;
        public int Xp => xp;    

        private void Awake() 
        {
            if(Instance == null)
               Instance = this;
            else
                Destroy(gameObject);

            uI = GetComponent<MissionManagerUI>();  

            Mission.updateMission                   += OnMissionUpdated;
            Mission.completeMission                 += OnCompleteMission;
            MainMissionSliderUI.OnAttractorInit     += OnAttractorInit;

        }

        private void Start() 
        {
            for(int i = 0; i < missionDatas.Length; i++)
                activeMissions.Add(new Mission(missionDatas[i]));

            uI.Init(activeMissions.ToArray());
        }

        private void Destroy()
        {
            Mission.updateMission                   -= OnMissionUpdated;
            Mission.completeMission                 -= OnCompleteMission;
            MainMissionSliderUI.OnAttractorInit     -= OnAttractorInit;
        }

        private void OnAttractorInit(UIParticleAttractor _attractor)
        {
            uIParticleAttractor = _attractor;
            uIParticleAttractor.onAttracted.AddListener(OnCollectedParticleAttracted);
        }

        public void HandleMissionClaimed(int index)
        {
            Mission mission = activeMissions[index];
            mission.Claim();

            int particleCount = mission.Data.RewardXp;

            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            ParticleSystem collectParticlesInstance = Instantiate(missionCollectParticles, screenCenter, Quaternion.identity, particleParent);

            uIParticleAttractor.AddParticleSystem(collectParticlesInstance);    

            collectParticlesInstance.emission.SetBurst(0, new ParticleSystem.Burst(0, particleCount));
            collectParticlesInstance.Play();
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

        private void OnCollectedParticleAttracted()
        {
            xp++;
            xpUpdated?.Invoke(xp);
        }

    }
}


