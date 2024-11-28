using UnityEngine;

[RequireComponent(typeof(CharacterHealth))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterLevel))]
[RequireComponent(typeof(CharacterDetection))]
[RequireComponent(typeof(CharacterWeapon))]
[RequireComponent(typeof(CharacterObjects))]
[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(CharacterAnimator))]
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    [Header("COMPONENTS:")]
    private CharacterLevel level;
    private CharacterAnimator anim;
    public CharacterWeapon weapon { get; private set; }
    public CharacterHealth health { get; private set; }
    [SerializeField] private CircleCollider2D _col;
    [SerializeField] private SpriteRenderer _sr;
    

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

        CharacterSelectionManager.OnCharacterSelected += CharacterSelectionCallback;
    }

    private void OnDestroy() =>  CharacterSelectionManager.OnCharacterSelected -= CharacterSelectionCallback;

    public void TakeDamage(int _damage) => health.TakeDamage(_damage);

    public Vector2 GetColliderCenter() => (Vector2)transform.position + _col.offset;

    public bool HasLeveledUp() => level.HasLeveledUp();

    private void CharacterSelectionCallback(CharacterDataSO _characterData)
    {
        StatisticsManager.Instance.RecordCharacterUsage(_characterData.ID);
        _sr.sprite = _characterData.Icon;
    } 
        
}
