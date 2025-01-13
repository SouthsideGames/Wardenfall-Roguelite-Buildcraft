using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameCardUI : MonoBehaviour
{
    [Header("UI ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private Image overlay;
    [SerializeField] private TextMeshProUGUI timer;

    private CardSO cardSO;
    private float activeTimer;
    private float cooldownTimer;
    private bool isActive;
    private bool isCoolingDown;

    public void Configure(CardSO _cardSO)
    {
        cardSO = _cardSO;

        activeTimer = 0;
        cooldownTimer = 0;
        isActive = false;
        isCoolingDown = false;

        if (icon != null)
            icon.sprite = cardSO.Icon;

        Debug.Log($"Card {cardSO.CardName} configured. Ready to use.");
        UpdateCardUI();
    }

    private void Update()
    {
        if (cardSO == null)
            return;

        if (isActive)
        {
            activeTimer -= Time.deltaTime;
            overlay.fillClockwise = true;

            if (activeTimer <= 0)
            {
                activeTimer = 0;
                isActive = false;
                CardEffect.Instance.DisableEffect(cardSO.EffectType);

                // Start cooldown
                cooldownTimer = cardSO.CooldownTime;
                isCoolingDown = true;
                Debug.Log($"Card {cardSO.CardName} effect ended. Cooldown started.");
            }
        }
        else if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;
            overlay.fillClockwise = false;

            if (cooldownTimer <= 0)
            {
                cooldownTimer = 0;
                isCoolingDown = false;
                Debug.Log($"Card {cardSO.CardName} cooldown ended. Ready to use.");
            }
        }

        UpdateCardUI();
    }

    public void OnCardButtonPressed()
    {
      Debug.Log($"Button for card {cardSO?.CardName} pressed."); // Add this line

        if (!isActive && !isCoolingDown)
        {
            activeTimer = cardSO.ActiveTime;
            isActive = true;
            CardEffect.Instance.ActivateEffect(cardSO.EffectType, cardSO.ActiveTime);
            Debug.Log($"Card {cardSO.CardName} activated for {cardSO.ActiveTime} seconds.");
        }
        else
        {
            Debug.Log($"Card {cardSO.CardName} is not ready to use. Active: {isActive}, Cooling down: {isCoolingDown}");
        }
    }

    private void UpdateCardUI()
    {
        if (isActive)
        {
            overlay.fillAmount = activeTimer / cardSO.ActiveTime;
            timer.text = $"{Mathf.CeilToInt(activeTimer)}s";
        }
        else if (isCoolingDown)
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

