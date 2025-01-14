using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameCardUI : MonoBehaviour
{
     [Header("UI ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private Image overlay;
    [SerializeField] private TMPro.TextMeshProUGUI timer;
    private CardSO cardSO;
    private float activeTimer;
    private float cooldownTimer;

    public void Configure(CardSO _cardSO)
    {
        cardSO = _cardSO;

        if (icon != null)
            icon.sprite = cardSO.Icon;

        UpdateCardUI();
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
            activeTimer = cardSO.ActiveTime;
            CardEffect.Instance.ActivateEffect(cardSO.EffectType, cardSO.ActiveTime);
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

