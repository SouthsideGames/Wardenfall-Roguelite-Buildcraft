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
            if (cardSO.ActiveTime > 0)
            {
                // Start the active timer for cards with active time
                activeTimer = cardSO.ActiveTime;
                CardEffect.Instance.ActivateEffect(cardSO.EffectType, cardSO.ActiveTime, cardSO);
            }
            else
            {
                // One-shot card: directly activate and start cooldown
                CardEffect.Instance.ActivateEffect(cardSO.EffectType, 0, cardSO); // Pass 0 since it's a one-shot
                cooldownTimer = cardSO.CooldownTime;
            }
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
        else if (CardEffect.Instance.IsEffectActive(cardSO.EffectType))
        {
            overlay.fillAmount = 0;
            timer.text = "Active";
        }
        else
        {
            overlay.fillAmount = 0;
            timer.text = "Ready";
        }
    }

    public bool IsCurrentCard()
    {
        return CardEffect.Instance.IsEffectActive(cardSO.EffectType);
    }

    public void ResetCooldown()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer = 0;
            Debug.Log($"{cardSO.CardName} cooldown reset.");
        }
    }

}

