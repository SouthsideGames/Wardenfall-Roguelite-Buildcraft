using UnityEngine;

[RequireComponent(typeof(CharacterHealth))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterLevel))]
[RequireComponent(typeof(CharacterDetection))]
[RequireComponent(typeof(CharacterWeapon))]
[RequireComponent(typeof(CharacterObjects))]
[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(CharacterCards))]
[RequireComponent(typeof(CharacterAnimator))]
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    [Header("COMPONENTS:")]
    private CharacterLevel level;
    private CharacterAnimator anim;
    public CharacterWeapon weapon { get; private set; }
    public CharacterHealth health { get; private set; }
    public CharacterStats stats { get; private set; }
    public CharacterController controller { get; private set; }
    public CharacterCards cards {get; private set; }
    [SerializeField] private CircleCollider2D _col;
    public SpriteRenderer _sr {get; private set; }
    private CharacterDataSO characterData;
    public CharacterDataSO CurrentCharacter => characterData;
    
    
    private void Awake()
    {
        if(Instance == null)
           Instance = this;
        else
            Destroy(gameObject);

        health = GetComponent<CharacterHealth>();  
        level = GetComponent<CharacterLevel>();
        weapon = GetComponent<CharacterWeapon>(); 
        anim = GetComponent<CharacterAnimator>();  
        stats = GetComponent<CharacterStats>();
        controller = GetComponent<CharacterController>(); 
        cards = GetComponent<CharacterCards>();
        _sr = GetComponentInChildren<SpriteRenderer>();

        CharacterSelectionManager.OnCharacterSelected += CharacterSelectionCallback;
    }

    private void OnDestroy() =>  CharacterSelectionManager.OnCharacterSelected -= CharacterSelectionCallback;
    public void TakeDamage(int _damage) => health.TakeDamage(_damage);
    public Vector2 GetColliderCenter() => (Vector2)transform.position + _col.offset;
    public bool HasLeveledUp() => level.HasLeveledUp();

    private void CharacterSelectionCallback(CharacterDataSO _characterData)
    {
        characterData = _characterData;
        StatisticsManager.Instance.RecordCharacterUsage(_characterData.ID);
        _sr.sprite = _characterData.Icon;
    } 

    public Vector2 GetAimDirection()
    {
        Vector2 aimDirection = controller.MoveDirection;

        if (aimDirection == Vector2.zero)
            aimDirection = Vector2.right; 

        return aimDirection.normalized;
    }
        
}
