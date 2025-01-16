using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameCardUI : MonoBehaviour
{
    [Header("UI ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private Image overlay;
    [SerializeField] private TextMeshProUGUI timer;

    [Header("BACKGROUND ELEMENTS:")]
    [SerializeField] private Image background;
    [SerializeField] private Sprite commonBackgroundImage;
    [SerializeField] private Sprite uncommonBackgroundImage;
    [SerializeField] private Sprite rareBackgroundImage;
    [SerializeField] private Sprite epicBackgroundImage;
    [SerializeField] private Sprite legendaryBackgroundImage;
    [SerializeField] private Sprite mythicBackgroundImage;
    [SerializeField] private Sprite exaltedBackgroundImage;

    private CardSO cardSO;
    private float activeTimer;
    private float cooldownTimer;

    public void Configure(CardSO _cardSO)
    {
        cardSO = _cardSO;

        if(_cardSO.Rarity == CardRarityType.Common)
           background.sprite = commonBackgroundImage;

        if (icon != null)
            icon.sprite = cardSO.Icon;

        ResetTimers();
        UpdateCardUI();
    }

    private void ResetTimers()
    {
        activeTimer = 0;
        cooldownTimer = 0;
    }

    private void Update()
    {
        if (cardSO == null)
            return;

        if (activeTimer > 0)
        {
            activeTimer -= Time.deltaTime;
            if (activeTimer <= 0)
            {
                CardEffect.Instance.DisableEffect(cardSO.EffectType);
                cooldownTimer = cardSO.CooldownTime;
            }
        }
        else if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        UpdateCardUI();
    }

    public void OnCardButtonPressed()
    {
        if (activeTimer <= 0 && cooldownTimer <= 0)
        {
            CardEffect.Instance.ActivateEffect(cardSO);
            activeTimer = cardSO.ActiveTime;
        }
    }

    private void UpdateCardUI()
    {
        if (activeTimer > 0)
        {
            overlay.fillAmount = activeTimer / cardSO.ActiveTime;
            timer.text = $"{Mathf.CeilToInt(activeTimer)}s";
        }
        else if (cooldownTimer > 0)
        {
            overlay.fillAmount = 1 - (cooldownTimer / cardSO.CooldownTime);
            timer.text = $"{Mathf.CeilToInt(cooldownTimer)}s";
        }
        else
        {
            overlay.fillAmount = 0;
            timer.text = "Ready";
        }
    }
}

