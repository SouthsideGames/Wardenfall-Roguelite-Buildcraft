using System;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

namespace SouthsideGames.DailyMissions
{
    using SouthsideGames.SaveManager;
    public class MainMissionSliderUI : MonoBehaviour
    {
        public static Action<UIParticleAttractor> OnAttractorInit;
        public static Action<RewardEntryData[]> rewarded;

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
        private List<SliderItemUI> sliderItems = new List<SliderItemUI>();
        private int lastRewardIndex;
        private bool[] rewardOpened;
        private const string lastRewardIndexKey     = "MissionLastRewardIndex";
        private const string rewardOpenedKey         = "MissionRewardsOpened"; 

        private void Awake()
        {
            MissionManager.xpUpdated    += OnXpUpdated;
            MissionManager.reset        += ResetSelf;
        }
        private void OnDestroy()
        {
            MissionManager.xpUpdated    -= OnXpUpdated;
            MissionManager.reset        -= ResetSelf;
        }

        private IEnumerator Start() 
        {
            yield return null;

            lastRewardIndex = 0;
            rewardOpened = new bool[data.RewardMilestoneDatas.Length];

            Load();
            Init();
        }

        private void Init()
        {
            GenerateSliderItems();
            InitSlider();
            UpdateVisuals(MissionManager.Instance.Xp);

            CheckForUnopenedReward();
        }

        private void CheckForUnopenedReward()
        {
            for (int i = 0; i < sliderItems.Count; i++)
            {
                if(!rewardOpened[i] && slider.value >= data.RewardMilestoneDatas[i].requiredXP)
                   sliderItems[i].Animate();
            }
        }

        private void GenerateSliderItems()
        {
            itemsParent.Clear();
            sliderItems.Clear();

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

                sliderItems.Add(itemInstance);
            }

            PlaceItems();

        }

        private void HandleSliderItemPressed(int _index)
        {
            bool canOpen = lastRewardIndex > _index;
            bool isOpened = rewardOpened[_index];

            if(!canOpen || isOpened)
              return;

            OpenReward(_index);
        }

        private void OpenReward(int _index)
        {
            rewardOpened[_index] = true;
            
            sliderItems[_index].StopAnimation();

            MissionRewardPopUpUI popup = PopUpManager.Show(rewardPopUp) as MissionRewardPopUpUI;
            popup.Configure(data.RewardMilestoneDatas[_index].rewards);

            Save();

            rewarded?.Invoke(data.RewardMilestoneDatas[_index].rewards);

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
            UpdateVisuals(_xp);

            CheckForRewards();
        }

        private void UpdateVisuals(int _xp)
        {
            slider.value = _xp;
            xpText.text = _xp.ToString();
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
            sliderItems[lastRewardIndex].Animate();

            lastRewardIndex++;
        }

        private void Load()
        {
            if(SaveManager.TryLoad(this, lastRewardIndexKey, out object _lastRewardIndex))
                lastRewardIndex = (int)_lastRewardIndex;

            if(SaveManager.TryLoad(this, rewardOpenedKey, out object _rewardsOpened))
                rewardOpened = (bool[])_rewardsOpened;
        }

        private void Save()
        {
            SaveManager.Save(this, lastRewardIndexKey, lastRewardIndex);
            SaveManager.Save(this, rewardOpenedKey, rewardOpened);
        }

        private void ResetSelf()
        {
            SaveManager.Remove(this, lastRewardIndexKey);
            SaveManager.Remove(this, rewardOpenedKey);

            CoroutineRunner.Instance.RunPooled(Start());
        }
    }

}
