using UnityEngine;

public class CardDraftUI : MonoBehaviour
{
    public static CardDraftUI Instance;

    private Action<CardSO> onCardChosen;
    private CardSO pendingReplacementCard;
    private List<CardSO> currentDeck;
    private int currentDeckCost;
    private int maxDeckCost;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowCardDraft(List<CardSO> draftOptions, Action<CardSO> onSelectCallback)
    {
        onCardChosen = onSelectCallback;
        // TODO: Spawn UI card buttons and hook them up to OnCardSelected()
        Debug.Log(\"[DraftUI] Showing card draft with \" + draftOptions.Count + \" options.\");
    }
}
