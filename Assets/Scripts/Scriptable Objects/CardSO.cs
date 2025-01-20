using System.Collections.Generic;
using UnityEngine;

 [CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Objects/New Card Data", order = 6)]
    public class CardSO : ScriptableObject
    {
    [Header("ELEMENTS:")]
    [SerializeField] private string iD;
    public string ID => iD;
    [SerializeField] private string cardName;
    public string CardName => cardName;
    [SerializeField] private string description;
    public string Description => description;
    [SerializeField] private Sprite icon;
    public Sprite Icon => icon;
    [SerializeField] private int cost;
    public int Cost => cost;
    [SerializeField] private CardType cardType;
    public CardType CardType => cardType;
    [SerializeField] private CardEffectType effectType;
    public CardEffectType EffectType => effectType;
    [SerializeField] private Sprite effectIcon;
    public Sprite EffectIcon => effectIcon;
    [SerializeField] private CardRarityType rarity;
    public CardRarityType Rarity => rarity;

    [Header("SETTINGS")]
    [SerializeField] private bool isCollected;
    public bool IsCollected => isCollected;
    [SerializeField] private bool isAutoActivated;
    public bool IsAutoActivated => isAutoActivated;
    [SerializeField] private bool isActive = true;
    public bool IsActive => isActive;

    [Header("TIMERS")]
    [SerializeField] private int effectValue;
    public int EffectValue => effectValue;
    [SerializeField] private float activeTime = 5f;
    public float ActiveTime => activeTime;
    [SerializeField] private float cooldownTime = 10f;
    public float CooldownTime => cooldownTime;

    public void Deactivate()
    {
        isActive = false;
    }

    public void Collected()
    {
        isCollected = true;
        SouthsideGames.SaveManager.SaveManager.Save(this, iD, isCollected);
    }
}


