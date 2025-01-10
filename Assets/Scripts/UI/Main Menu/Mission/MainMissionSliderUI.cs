using System;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;

namespace SouthsideGames.DailyMissions
{
    public class MainMissionSliderUI : MonoBehaviour
    {
        public static Action<UIParticleAttractor> OnAttractorInit;

        [Header("ELEMENTS:")]
        [SerializeField] private Slider slider;
        [SerializeField] private SliderItemUI uIAttractorItemPrefab;
        [SerializeField] private SliderItemUI sliderItemPrefab;
        [SerializeField] private Transform itemsParent;

        [Header("DATA:")]
        [SerializeField] private RewardGroupDataSO data;
        [SerializeField] private Sprite currencyIcon;

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
            GenerateSliderItems();
            InitSlider();
        }

        private void GenerateSliderItems()
        {
            itemsParent.Clear();

            SliderItemUI attractorItem = Instantiate(uIAttractorItemPrefab, itemsParent);
            attractorItem.Configure(currencyIcon, 0.ToString());

            OnAttractorInit?.Invoke(attractorItem.GetComponent<UIParticleAttractor>());


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
