using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GearRoomCardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    private CardSO cardData;
    private GearShopManager manager;

    public void Initialize(CardSO data, GearShopManager mgr)
    {
        cardData = data;
        manager = mgr;
        icon.sprite = data.icon;
        nameText.text = data.cardName;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        manager.ShowCardDetail(cardData);
    }
}
