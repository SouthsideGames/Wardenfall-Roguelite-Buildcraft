using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Objects/New Card Data", order = 6)]
public class CardSO : ScriptableObject
{
    public string cardName;
    public string description;
    public Sprite icon;
    public CardEffectType effectType;
    public CardRarity rarity;
    public float effectValue;
    public float activeTime;
    public int level;
    public int cost;
    public bool isUnlocked;
    public string unlockHint;
}
