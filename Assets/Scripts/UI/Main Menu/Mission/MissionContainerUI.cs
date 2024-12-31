using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace SouthsideGames.DailyMissions
{
    public class MissionContainerUI : MonoBehaviour
    {
        [Header("REWARD SECTION ELEMENTS:")]
        [SerializeField] private Image rewardImage;
        [SerializeField] private TextMeshProUGUI rewardText;

        [Header("SLIDER ELEMENTS:")]
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI progressText;

        [Header("BUTTON SECTION")]
        [SerializeField] private GameObject inProgress;
        [SerializeField] private Button claimButton;
        [SerializeField] private GameObject checkIcon;

        private Mission mission;

        public void Configure(Mission _mission, Action _callback)
        {
            mission = _mission; 

            rewardImage.sprite = _mission.Data.Icon;
            rewardText.text = "x" + _mission.Data.RewardXp;
            labelText.text = GetMissionLabel(_mission.Data);

            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(() => _callback?.Invoke());

            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            progressText.text = mission.ProgressString;
            progressSlider.value = mission.Progress;


            if(mission.Progress >= 1)
                CompleteMission();
            else
                UnCompleteMission();
        }

        private void CompleteMission()
        {
            inProgress.SetActive(false);
            checkIcon.SetActive(false);
            claimButton.gameObject.SetActive(true);
        }

        private void UnCompleteMission()
        {
            inProgress.SetActive(true);
            checkIcon.SetActive(false);
            claimButton.gameObject.SetActive(false);
        }

        public void ClaimMission()
        {
            
            checkIcon.SetActive(true);
            claimButton.gameObject.SetActive(false);
            inProgress.SetActive(false);
        }


        private string GetMissionLabel(MissionDataSO _data)
        {
            switch (_data.Type)
            {
                case MissionType.survivalPlayed:
                    return $"Play {_data.Target} times";
                case MissionType.waveBasedPlayed:
                    return $"Play {_data.Target} times";
                case MissionType.bossRushPlayed:
                    return $"Play {_data.Target} times";
                case MissionType.enemiesPopped:
                    return $"Defeat {_data.Target} enemies";
                case MissionType.wavesCompleted:
                    return $"Complete {_data.Target} waves";
                case MissionType.currencyCollected:
                    return $"Collect {_data.Target} meat";
                case MissionType.premiumCurrencyCollected:
                    return $"Collect {_data.Target} cash";
                case MissionType.gemCollected:
                    return $"Collect {_data.Target} gems";
                default:
                    Debug.LogError("No label for this mission type : " + _data.Type);
                    return "";
            }
        }
    }
}

