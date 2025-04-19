using SouthsideGames.DailyMissions;
using UnityEngine;
using UnityEngine.UI;

public class MissionRewardPopUpUI : PopupBaseUI
{
    [Header("ANIMATIONS:")]
    [SerializeField] private Image background;
    [SerializeField] private RectTransform mainContainer;
    [SerializeField] private float animationDuration;
    [SerializeField] private LeanTweenType openEase;

    [Header("INTERACTABLE:")]
    [SerializeField] private Button closeButton;

    [Header("ELEMENTS:")]
    [SerializeField] private MissionRewardContainerUI rewardContainerPrefab;
    [SerializeField] private Transform rewardContainersParent;
    [SerializeField] private Sprite placeholderIcon;

    private bool isClosing;


    void Start()
    {
        closeButton.onClick.RemoveAllListeners();   
        closeButton.onClick.AddListener(Close); 

        Open();
    }

    public void Configure(RewardEntryData[] rewards)
    {
        for(int i = 0; i < rewards.Length; i++)
        {
            RewardEntryData data = rewards[i];

            MissionRewardContainerUI containerInstance = Instantiate(rewardContainerPrefab, rewardContainersParent);
            containerInstance.Configure(RewardSpriteMapper.GetSprite(data.type), data.amount.ToString());
        }
    }

    private void Open()
    {
        float backgroundTargetAlpha = background.color.a;
        background.color = Color.clear;

        LeanTween.alpha(background.rectTransform, backgroundTargetAlpha, animationDuration).setRecursive(false);

        mainContainer.localScale = Vector3.zero;
        LeanTween.scale(mainContainer, Vector3.one, animationDuration).setEase(openEase);   
    }

    private void Close()
    {
        if(isClosing)
           return;

        isClosing = true;

        Destroy(gameObject);    
    }

    private void Destroy() => closed?.Invoke(this);
}

