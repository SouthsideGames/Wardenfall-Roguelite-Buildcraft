using System;
using System.Security.Claims;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        public bool IsClaimed => mission.IsClaimed;

        public void Configure(Mission _mission, Action _callback)
        {
            mission = _mission; 

            rewardImage.sprite = _mission.Data.Icon;
            rewardText.text = "x" + _mission.Data.RewardXp;
            labelText.text = MissionDescriptionMapper.GetDescription(mission.Data);

            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(() => _callback?.Invoke());

            UpdateVisuals();

            if(mission.IsClaimed)
                ClaimMission();
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

    }
}

