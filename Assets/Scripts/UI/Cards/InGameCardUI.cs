using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameCardUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private Image overlay;
    [SerializeField] private TextMeshProUGUI timer;

    public CardSO cardSO;


    public void Configure(CardSO _cardSO)
    {
        cardSO = _cardSO;

        
    }
}
