using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIExtensions;

namespace SouthsideGames.DailyMissions
{
    using SouthsideGames.SaveManager;

    [RequireComponent(typeof(MissionManagerUI))]
    [RequireComponent(typeof(MissionTimer))]
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance;
        public static Action<int> xpUpdated;
        public static Action reset;

        [Header("COMPONENTS:")]
        private MissionManagerUI uI;
        private MissionTimer timer;
        [SerializeField] private GameObject alertObject;

        [Header("DATA:")]
        [SerializeField] private MissionDataSO[] missionDatas;
        List<Mission> activeMissions = new List<Mission>(); 

        [Header("EFFECT:")]
        [SerializeField] private ParticleSystem missionCollectParticles;
        [SerializeField] private Transform particleParent;
        private UIParticleAttractor uIParticleAttractor;

        private int xp;
        public int Xp => xp;    
        private bool shouldSave;
        private int[] amounts;
        private bool[] claimedStates;
        private const string amountsKey         = "MissionDataAmounts";
        private const string claimedStatesKey   = "MissionClaimedStates";
        private const string xpkey              = "MissionXp";
        private const string alertKey           = "MissionAlertState";

        private void Awake() 
        {
            if(Instance == null)
               Instance = this;
            else
                Destroy(gameObject);

            uI = GetComponent<MissionManagerUI>();  
            timer = GetComponent<MissionTimer>();   

            Mission.updateMission                   += OnMissionUpdated;
            Mission.completeMission                 += OnCompleteMission;
            MainMissionSliderUI.OnAttractorInit     += OnAttractorInit;

            CoroutineRunner.Instance.RunPooled(SaveCoroutine());

        }

        private void Start() 
        {
            Load();

            for(int i = 0; i < missionDatas.Length; i++)
                activeMissions.Add(new Mission(missionDatas[i], amounts[i], claimedStates[i]));

            uI.Init(activeMissions.ToArray());
            timer.Init(this);
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

            int particleCount = mission.Data.RewardXP;

            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            ParticleSystem collectParticlesInstance = Instantiate(missionCollectParticles, screenCenter, Quaternion.identity, particleParent);

            uIParticleAttractor.AddParticleSystem(collectParticlesInstance);    

            collectParticlesInstance.emission.SetBurst(0, new ParticleSystem.Burst(0, particleCount));
            collectParticlesInstance.Play();

            Save();
        }

        private void OnMissionUpdated(Mission _mission)
        {
            uI.UpdateMission(activeMissions.IndexOf(_mission));
        }
        
        private void OnCompleteMission(Mission _mission)
        {
            alertObject.SetActive(true);
            SaveManager.Save(this, alertKey, true);
        }

        public static void Increment(MissionType _missionType, int _amount)
        {
            bool incremented = false;

            for (int i = 0; i < Instance.activeMissions.Count; i++)
            {
                if(Instance.activeMissions[i].IsComplete || Instance.activeMissions[i].IsClaimed)
                    continue;
                    
                if(Instance.activeMissions[i].Type == _missionType)
                   Instance.activeMissions[i].Amount += _amount;

                incremented = true;
            }

            if(incremented)
                Instance.Save();
        }

        private void OnCollectedParticleAttracted()
        {
            xp++;
            xpUpdated?.Invoke(xp);

            shouldSave = true;
        }

        public void ResetMissions()
        {
            amounts = new int[missionDatas.Length];
            claimedStates = new bool[missionDatas.Length];
            xp = 0;

            SaveManager.Remove(this, amountsKey);
            SaveManager.Remove(this, claimedStatesKey);
            SaveManager.Remove(this, xpkey);

            activeMissions.Clear(); 

            for (int i = 0; i < missionDatas.Length; i++)
               activeMissions.Add(new Mission(missionDatas[i]));

            uI.Init(activeMissions.ToArray());

            timer.ResetSelf();

            alertObject.SetActive(true);
            SaveManager.Save(this, alertKey, true);

            reset?.Invoke();
        }

        private void Load()
        {
            amounts = new int[missionDatas.Length];
            claimedStates = new bool[missionDatas.Length];

            if(SaveManager.TryLoad(this, amountsKey, out object _amounts))
               amounts = (int[])_amounts;

            if(SaveManager.TryLoad(this, claimedStatesKey, out object _claimedStates))
               claimedStates = (bool[])_claimedStates;

            if(SaveManager.TryLoad(this, xpkey, out object _xp))
               xp = (int)_xp;

            if (SaveManager.TryLoad(this, alertKey, out object alertState))
            {
                alertObject.SetActive((bool)alertState);
            }
            else
            {
                alertObject.SetActive(true);
                SaveManager.Save(this, alertKey, true);
            }

    
        }

        private void Save()
        {
            for(int i = 0; i < activeMissions.Count; i++)
            {
                amounts[i] = activeMissions[i].Amount;
                claimedStates[i] = activeMissions[i].IsClaimed;
            }

            SaveManager.Save(this, amountsKey, amounts);
            SaveManager.Save(this, claimedStatesKey, claimedStates);
            SaveManager.Save(this, xpkey, xp);
        }
        
        IEnumerator SaveCoroutine()
        {
            while(true)
            {
                yield return new WaitForSeconds(5);

                if(shouldSave)
                {
                    shouldSave = false;
                    Save();
                }
                   
            }
        }

        public void RemoveAlert()
        {
            alertObject.SetActive(false);
            SaveManager.Save(this, alertKey, false);
        }

    }
}


