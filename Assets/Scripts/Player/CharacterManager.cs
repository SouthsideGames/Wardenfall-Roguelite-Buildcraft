using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterHealth))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterLevel))]
[RequireComponent(typeof(CharacterDetection))]
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    [Header("COMPONENTS:")]
    private CharacterHealth characterHealth;
    private CharacterLevel characterLevel;
    [SerializeField] private CircleCollider2D _col;
    

    private void Awake()
    {
        if(Instance == null)
           Instance = this;
        else
            Destroy(gameObject);

        characterHealth = GetComponent<CharacterHealth>();  
        characterLevel = GetComponent<CharacterLevel>();    
    }


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
}
