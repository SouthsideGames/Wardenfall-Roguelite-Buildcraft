using System.Collections.Generic;
using UnityEngine;

public class CardDraftManager : MonoBehaviour, IGameStateListener
{
     [SerializeField] private GameObject panel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private CardOptionUI cardOptionPrefab;
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private int optionsToShow = 3;

    public void GameStateChangedCallback(GameState state)
    {
        if (state == GameState.CardDraft)
            OpenCardDraft();
        else
            CloseCardDraft();
    }

    private void OpenCardDraft()
    {
        panel.SetActive(true);

        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        List<CardSO> available = new List<CardSO>(cardLibrary.allCards);
        int displayCount = Mathf.Min(optionsToShow, available.Count);

        for (int i = 0; i < displayCount; i++)
        {
            int index = Random.Range(0, available.Count);
            CardSO selected = available[index];
            available.RemoveAt(index);

            CardOptionUI card = Instantiate(cardOptionPrefab, cardContainer);
            card.Configure(selected, () => OnCardSelected(selected));
        }
    }

    private void CloseCardDraft() => panel.SetActive(false);

    private void OnCardSelected(CardSO card)
    {
        CharacterManager.Instance.cards.AddCard(card);
        GameManager.Instance.StartTraitSelection(); 
    }
}
