using System;
using UnityEngine;
using UnityEngine.UI;

namespace SouthsideGames.DailyMissions
{
    public class MainMissionSliderUI : MonoBehaviour
    {
        [Header("ELEMENTS:")]
        [SerializeField] private Slider slider;

        [Header("DATA:")]
        [SerializeField] private RewardGroupDataSO data;

        private void Awake() 
        {
            MissionManager.xpUpdated += OnXpUpdated;
        }

        private void OnDestroy()
        {
            MissionManager.xpUpdated -= OnXpUpdated;
        }

        private void Start() 
        {
            Init();
        }

        private void Init()
        {
            InitSlider();
        }

        private void InitSlider()
        {
            slider.minValue = 0;
            slider.maxValue = data.RewardMilestoneDatas[data.RewardMilestoneDatas.Length - 1].requiredXP;

            slider.value = 0;
        }

        private void OnXpUpdated(int _xp)
        {
            slider.value = _xp;
        }
    }

}
