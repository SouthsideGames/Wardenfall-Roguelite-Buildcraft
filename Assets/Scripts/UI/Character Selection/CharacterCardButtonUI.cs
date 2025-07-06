using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCardButtonUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameLabel;

    private CardSO cardData;
    private Action<CardSO> onClickCallback;

    public void Initialize(CardSO card, Action<CardSO> clickCallback)
    {
        cardData = card;
        iconImage.sprite = card.icon;
        nameLabel.text = card.cardName;
        onClickCallback = clickCallback;

        GetComponent<Button>().onClick.AddListener(OnPressed);
    }

    private void OnPressed() => onClickCallback?.Invoke(cardData);
}
