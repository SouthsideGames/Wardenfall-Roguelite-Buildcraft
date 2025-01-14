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
        [SerializeField] private bool isCollected;   
        public bool IsCollected => isCollected; 

        [Header("TIMERS")]
        [SerializeField] private float activeTime = 5f;
        public float ActiveTime => activeTime;  
        [SerializeField] private float cooldownTime = 10f;
        public float CooldownTime => cooldownTime;

       
        public void Collected()
        {
            isCollected = true;

            SouthsideGames.SaveManager.SaveManager.Save(this, iD, isCollected);
        }
    }
