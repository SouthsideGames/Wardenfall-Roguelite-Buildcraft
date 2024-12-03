using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniIconUI : MonoBehaviour
{
     [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI costText;

    private CardSO cardData;

    public void Configure(Sprite icon, int cost)
    {
        iconImage.sprite = icon;
        costText.text = cost.ToString();
    }

    public bool MatchesCard(CardSO card) => cardData == card;
}
