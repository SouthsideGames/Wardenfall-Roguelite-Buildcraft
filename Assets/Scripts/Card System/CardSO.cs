using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Objects/New Card Data", order = 6)]
public class CardSO : ScriptableObject
{
    [Header("IDENTITY")]
    public string cardID;
    public string cardName;
    [TextArea] public string description;
    public Sprite icon;
    public string[] synergyWith;
    public CardType cardType;

    [Header("TYPE & EFFECT")]
    public CardEffectType effectType;
    public CardRarity rarity;
    public float effectValue;
    public float activeTime;

    [Header("DECK & UI")]
    public int cost;
    public float cooldown;
    public GameObject effectPrefab;
    public bool cooldownStartsOnUse = false;

    [Header("UNLOCKING")]
    public CardUnlockData unlockData;
}

[System.Serializable]
public class CardUnlockData
{
    public bool unlocked = false;
    public int unlockCost = 0;
    public ItemRewardType currencyType = ItemRewardType.UnlockTickets;
}