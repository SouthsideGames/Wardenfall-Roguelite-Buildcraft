using System.Collections;
using System.Collections.Generic;
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

            UpdateCardUI();
        }

        private void UpdateCardUI()
        {
            if (cardSO.IsActive())
            {
           
                overlay.fillAmount = cardSO.GetActiveTimeRemaining() / cardSO.ActiveTime;
                timer.text = $"{Mathf.CeilToInt(cardSO.GetActiveTimeRemaining())}s";
            }
            else if (cardSO.IsCoolingDown())
            {
                
                overlay.fillAmount = 1 - (cardSO.GetCooldownRemaining() / cardSO.CooldownTime);
                timer.text = $"{Mathf.CeilToInt(cardSO.GetCooldownRemaining())}s";
            }
            else
            {
                
                overlay.fillAmount = 0;
                timer.text = "Ready";
            }
        }
    }

