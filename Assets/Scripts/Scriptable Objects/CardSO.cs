using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Objects/New Card Data", order = 6)]
public class CardSO : ScriptableObject
{
    [Header("ELEMENTS:")]
    [SerializeField] private string iD;
    public string ID => iD; 
    [SerializeField] private string cardName;  
    public string CardName => cardName;
    [SerializeField] private string effectName;
    public string EffectName => effectName;
    [SerializeField] private string description;   
    public string Description => description;      
    [SerializeField] private Sprite icon;    
    public Sprite Icon => icon;            
    [SerializeField] private int cost;
    public int Cost => cost;                
    [SerializeField] private CardEffectType effectType; 
    public CardEffectType EffectType => effectType; 
    [SerializeField] private float effectValue;     
    public float EffectValue => effectValue;   
    [SerializeField] private Sprite effectIcon;     
    public Sprite EffectIcon => effectIcon;  
    [SerializeField] private CardRarityType rarity;     
    public CardRarityType Rarity => rarity;    
    [SerializeField] private bool isCollected;   
    public bool IsCollected => isCollected; 

    [Header("TIMERS")]
    [SerializeField] private float activeTime = 5f;  // How long the effect lasts
    [SerializeField] private float cooldownTime = 10f; // How long before it can be reused
    private float activeEndTime;  // Timestamp when the active effect ends
    private float cooldownEndTime; // Timestamp when the cooldown ends 

    [Header("EXTRA:")]
    [SerializeField] private GameObject SpawnPrefab;    
    [SerializeField] private AudioClip ActivationSound;  

  /// <summary>
    /// Activates the card's effect, starting its active and cooldown timers.
    /// </summary>
    public void Activate(GameObject target)
    {
        if (IsCoolingDown())
        {
            Debug.LogWarning($"Card {CardName} is cooling down. Cooldown ends in {GetCooldownRemaining()} seconds.");
            return;
        }

        Debug.Log($"Activating {CardName} on {target.name}");
        activeEndTime = Time.time + activeTime;
        cooldownEndTime = activeEndTime + cooldownTime;

        switch (EffectType)
        {
            case CardEffectType.Damage:
                ApplyDamage(target, EffectValue);
                break;
            case CardEffectType.Utility:
                ApplyBuff(target, EffectValue);
                break;
            case CardEffectType.Support:
                SupportTarget(target, EffectValue);
                break;
            case CardEffectType.Summon:
                SummonPrefab(target);
                break;
            default:
                Debug.LogWarning("Unknown card effect type.");
                break;
        }

        if (ActivationSound)
            AudioManager.Instance.PlaySFX(ActivationSound);
    }

    public bool IsActive() => Time.time < activeEndTime;
    public bool IsCoolingDown() => Time.time < cooldownEndTime && Time.time >= activeEndTime;
    public float GetActiveTimeRemaining() => Mathf.Max(0, activeEndTime - Time.time);
    public float GetCooldownRemaining() => Mathf.Max(0, cooldownEndTime - Time.time);

    private void ApplyDamage(GameObject target, float damage)
    {
        Debug.Log($"Dealing {damage} damage to {target.name}");
        // Implement damage logic here
    }

    private void ApplyBuff(GameObject target, float buffValue)
    {
        Debug.Log($"Applying buff of {buffValue} to {target.name}");
        // Implement buff logic here
    }

    private void SupportTarget(GameObject target, float healAmount)
    {
        Debug.Log($"Healing {target.name} for {healAmount}");
        // Implement heal logic here
    }

    private void SummonPrefab(GameObject target)
    {
        if (SpawnPrefab)
        {
            Instantiate(SpawnPrefab, target.transform.position, Quaternion.identity);
            Debug.Log($"Summoned {SpawnPrefab.name} at {target.transform.position}");
        }
    }

    public void Collected()
    {
        isCollected = true;

        SouthsideGames.SaveManager.SaveManager.Save(this, iD, isCollected);
    }
}
