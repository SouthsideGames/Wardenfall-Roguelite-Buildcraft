using System;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SouthsideGames.DailyMissions
{
    public class MainMissionSliderUI : MonoBehaviour
    {
        public static Action<UIParticleAttractor> OnAttractorInit;

        [Header("ELEMENTS:")]
        [SerializeField] private Slider slider;
        [SerializeField] private SliderItemUI uIAttractorItemPrefab;
        [SerializeField] private SliderItemUI sliderItemPrefab;
        [SerializeField] private RectTransform itemsParent;
        [SerializeField] private MissionRewardPopUpUI rewardPopUp;
        private TextMeshProUGUI xpText;

        [Header("DATA:")]
        [SerializeField] private RewardGroupDataSO data;
        [SerializeField] private Sprite currencyIcon;
        private int lastRewardIndex;

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

            lastRewardIndex = 0;
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

            xpText = attractorItem.Text;

            OnAttractorInit?.Invoke(attractorItem.GetComponent<UIParticleAttractor>());

            for(int i = 0; i < data.RewardMilestoneDatas.Length; i++)
            {
                RewardMilestoneData milestoneData = data.RewardMilestoneDatas[i];

                SliderItemUI itemInstance = Instantiate(sliderItemPrefab, itemsParent);
                itemInstance.Configure(milestoneData.Icon, milestoneData.requiredXP.ToString()); 

                int _i = i;
                itemInstance.Button.onClick.AddListener(() => HandleSliderItemPressed(_i));
            }

            PlaceItems();

        }

        private void HandleSliderItemPressed(int _index)
        {
            bool canOpen = lastRewardIndex > _index;

            if(!canOpen)
              return;

            OpenReward(_index);
        }

        private void OpenReward(int _index)
        {
            itemsParent.GetChild(_index + 1).GetComponent<SliderItemUI>().StopAnimation();

            MissionRewardPopUpUI popup = PopUpManager.Show(rewardPopUp) as MissionRewardPopUpUI;
            popup.Configure(data.RewardMilestoneDatas[_index].rewards);



        }

        private void InitSlider()
        {
            slider.minValue = 0;
            slider.maxValue = data.RewardMilestoneDatas[data.RewardMilestoneDatas.Length - 1].requiredXP;

            slider.value = 0;
        }

        private void PlaceItems()
        {
            float width = itemsParent.rect.width;
            float spacing = width / (itemsParent.childCount - 1);

            Vector2 startPosition = (Vector2)itemsParent.position - Vector2.right * width / 2;

            for(int i = 0; i < itemsParent.childCount; i++)
                itemsParent.GetChild(i).position = startPosition + spacing * i * Vector2.right; 
        }

        private void OnXpUpdated(int _xp)
        {
            slider.value = _xp;
            xpText.text = _xp.ToString();

            CheckForRewards();
        }

        private void CheckForRewards()
        {
            if(lastRewardIndex > data.RewardMilestoneDatas.Length - 1)
                return;
                
            if(slider.value >= data.RewardMilestoneDatas[lastRewardIndex].requiredXP)
               EnableReward();
        }

        private void EnableReward()
        {
            SliderItemUI item = itemsParent.GetChild(lastRewardIndex + 1).GetComponent<SliderItemUI>();
            item.Animate();

            lastRewardIndex++;
        }
    }

}
