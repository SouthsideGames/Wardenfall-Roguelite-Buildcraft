using UnityEngine;

[RequireComponent(typeof(CharacterHealth))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterLevel))]
[RequireComponent(typeof(CharacterDetection))]
[RequireComponent(typeof(CharacterWeapon))]
[RequireComponent(typeof(CharacterObjects))]
[RequireComponent(typeof(CharacterStats))]
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    [Header("COMPONENTS:")]
    private CharacterHealth characterHealth;
    private CharacterLevel characterLevel;
    public CharacterWeapon characterWeapon { get; private set; }
    [SerializeField] private CircleCollider2D _col;
    [SerializeField] private SpriteRenderer characterRenderer;
    

    private void Awake()
    {
        if(Instance == null)
           Instance = this;
        else
            Destroy(gameObject);

        characterHealth = GetComponent<CharacterHealth>();  
        characterLevel = GetComponent<CharacterLevel>();
        characterWeapon = GetComponent<CharacterWeapon>();

        CharacterSelectionManager.OnCharacterSelected += CharacterSelectionCallback;
    }

    private void OnDestroy() =>  CharacterSelectionManager.OnCharacterSelected -= CharacterSelectionCallback;

    public void TakeDamage(int _damage)
    {
        characterHealth.TakeDamage(_damage);    
    }

    public Vector2 GetColliderCenter()
    {
        return (Vector2)transform.position + _col.offset;
    }

    public bool HasLeveledUp()
    {
        return characterLevel.HasLeveledUp();
    }

    private void CharacterSelectionCallback(CharacterDataSO _characterData)
    {
        //StatisticsManager.Instance.RecordCharacterUsage(_characterData.ID);
        characterRenderer.sprite = _characterData.Icon;
    } 
        
}
