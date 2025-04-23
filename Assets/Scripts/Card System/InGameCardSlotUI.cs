using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameCardSlotUI : MonoBehaviour
{
    [Header("ELEMENTS")]
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Button clickButton;

    [Header("RARITY BACKGROUNDS")]
    [SerializeField] private Sprite commonBackground;
    [SerializeField] private Sprite uncommonBackground;
    [SerializeField] private Sprite rareBackground;
    [SerializeField] private Sprite epicBackground;
    [SerializeField] private Sprite legendaryBackground;
    [SerializeField] private Sprite mythicBackground;
    [SerializeField] private Sprite exaltedBackground;

    private float cooldownRemaining = 0f;
    private bool isCoolingDown = false;
    private CardSO assignedCard;

    public void Setup(CardSO card)
    {
        assignedCard = card;
        icon.sprite = card.icon;
        SetRarityBackground(card.rarity);
        cooldownText.text = "";
        isCoolingDown = false;
        clickButton.interactable = true;

        clickButton.onClick.RemoveAllListeners();
        clickButton.onClick.AddListener(OnCardClicked);
    }

    private void OnCardClicked()
    {
        if (isCoolingDown) return;

        Debug.Log($"Activating card: {assignedCard.cardName}");

        CharacterManager.Instance
            .GetComponent<CardEffectManager>()
            .ActivateCard(assignedCard, this);
    }

    private void Update()
    {
        if (isCoolingDown)
        {
            cooldownRemaining -= Time.deltaTime;
            if (cooldownRemaining > 0f)
                cooldownText.text = Mathf.CeilToInt(cooldownRemaining).ToString();
            else
            {
                cooldownText.text = "";
                isCoolingDown = false;
                cooldownRemaining = 0f;
                clickButton.interactable = true; 
            }
        }
    }

    public void TriggerCooldown(float duration)
    {
        if (CharacterManager.Instance.cards.HasCard("card_cycle"))
        {
            duration *= 0.5f;
        }

        cooldownRemaining = duration;
        isCoolingDown = true;
        clickButton.interactable = false; // ✅ Disable button during cooldown
        cooldownText.text = Mathf.CeilToInt(duration).ToString();
    }

    private void SetRarityBackground(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common:
                background.sprite = commonBackground;
                break;
            case CardRarity.Uncommon:
                background.sprite = uncommonBackground;
                break;
            case CardRarity.Rare:
                background.sprite = rareBackground;
                break;
            case CardRarity.Epic:
                background.sprite = epicBackground;
                break;
            case CardRarity.Legendary:
                background.sprite = legendaryBackground;
                break;
            case CardRarity.Mythic:
                background.sprite = mythicBackground;
                break;
            case CardRarity.Exalted:
                background.sprite = exaltedBackground;
                break;
            default:
                background.sprite = commonBackground;
                break;
        }
    }

    public void ResetCooldown()
    {
        cooldownRemaining = 0f;
        isCoolingDown = false;
        cooldownText.text = "";
        clickButton.interactable = true;
    }

    public bool IsCoolingDown()
    {
        return isCoolingDown;
    }

} 
