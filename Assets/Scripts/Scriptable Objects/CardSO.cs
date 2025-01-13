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
        private float activeEndTime;  
        private float cooldownEndTime; 
        private bool isActive => Time.time < activeEndTime;
        private bool isCoolingDown => Time.time < cooldownEndTime;


        [Header("EXTRA:")]
        [SerializeField] private GameObject SpawnPrefab;    
        [SerializeField] private AudioClip ActivationSound;  

        public bool CanActivate() => !isActive && !isCoolingDown;

        public void ActivateEffect()
        {
            if (!CanActivate())
                return;

            activeEndTime = Time.time + activeTime;
            cooldownEndTime = Time.time + activeTime + cooldownTime;

            ExecuteEffect();
        }

        private void ExecuteEffect()
        {
            // Implement the specific effect logic based on the card type
            switch (cardType)
            {
                case CardType.Damage:
                    // Apply damage logic
                    Debug.Log($"{cardName} activated! Dealing damage.");
                    break;
                case CardType.Utility:
                    // Apply utility effect
                    Debug.Log($"{cardName} activated! Utility effect applied.");
                    break;
                case CardType.Support:
                    // Apply support effect
                    Debug.Log($"{cardName} activated! Support effect applied.");
                    break;
                case CardType.Summon:
                    // Summon objects
                    Debug.Log($"{cardName} activated! Summoning objects.");
                    break;
            }
        }

        public float GetActiveTimeRemaining() => Mathf.Max(0, activeEndTime - Time.time);

        public float GetCooldownRemaining() => Mathf.Max(0, cooldownEndTime - Time.time);

        public bool IsActive() => isActive;

        public bool IsCoolingDown() => isCoolingDown;
        public void Collected()
        {
            isCollected = true;

            SouthsideGames.SaveManager.SaveManager.Save(this, iD, isCollected);
        }
    }
