using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Objects/Cards/New Card Data", order = 0)]
public class CardSO : ScriptableObject
{
    [Header("ELEMENTS:")]
    [SerializeField] private string cardName;  
    public string CardName => cardName;          
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
    [SerializeField] private bool isCollected;   
    public bool IsCollected => isCollected;         

    [Header("EXTRA:")]
    [SerializeField] private GameObject SpawnPrefab;    
    [SerializeField] private AudioClip ActivationSound;  

    public void Activate(GameObject target)
    {
        Debug.Log($"Activating {CardName} on {target.name}");

        switch (EffectType)
        {
            case CardEffectType.Damage:
                ApplyDamage(target, EffectValue);
                break;
            case CardEffectType.Buff:
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

    // Example Effects
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

    public void Collected() => isCollected = true;
}
