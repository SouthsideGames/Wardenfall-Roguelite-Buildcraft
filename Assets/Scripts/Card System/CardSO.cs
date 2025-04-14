using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Objects/New Card Data", order = 6)]
public class CardSO : ScriptableObject
{
    [Header("IDENTITY")]
    public string cardID;
    public string cardName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("TYPE & EFFECT")]
    public CardEffectType effectType;
    public CardRarity rarity;
    public float effectValue;
    public float activeTime;
    public int level;

    [Header("DECK & UI")]
    public int cost;
    public float cooldown;

    [Header("UNLOCKING")]
    public bool isUnlocked;
    public string unlockHint;
}
