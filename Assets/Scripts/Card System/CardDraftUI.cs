using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDraftUI : MonoBehaviour
{
    public static CardDraftUI Instance;

    private Action<CardSO> onCardChosen;
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowCardDraft(List<CardSO> draftOptions, Action<CardSO> onSelectCallback) => onCardChosen = onSelectCallback;


    public void OnCardSelected(CardSO selectedCard) => onCardChosen?.Invoke(selectedCard);
     
}