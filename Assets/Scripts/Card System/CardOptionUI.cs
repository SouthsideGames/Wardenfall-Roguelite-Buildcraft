using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CardOptionUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button lockButton;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button selectButton;

    [HideInInspector] public CardSO card;
    private Action onSelected;
    private bool isLocked = false;
    public bool IsLocked => isLocked;

    public void SetCard(CardSO cardData, Action onClick, bool locked = false)
    {
        card = cardData;
        onSelected = onClick;
        isLocked = locked;

        iconImage.sprite = card.icon;
        nameText.text = card.cardName;
        descriptionText.text = card.description;
        costText.text = $"Cost: {card.cost}";

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelected?.Invoke());

        lockButton.onClick.RemoveAllListeners();
        lockButton.onClick.AddListener(ToggleLockState);

        SetLockVisual(isLocked);

        if (cardData.cost + CharacterManager.Instance.cards.currentTotalCost > CharacterManager.Instance.cards.GetEffectiveDeckCap())
        {
            costText.color = Color.red;
            selectButton.interactable = false;
        }
        else
        {
            costText.color = Color.white; 
            selectButton.interactable = true; 
        }
    }

    public void ToggleLockState()
    {
        isLocked = !isLocked;
        SetLockVisual(isLocked);
    }

    private void SetLockVisual(bool locked)
    {
        if (lockButton != null && lockButton.image != null)
        {
            lockButton.image.sprite = locked ? lockedSprite : unlockedSprite;
        }
    }
}