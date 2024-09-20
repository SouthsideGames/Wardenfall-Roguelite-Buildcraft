using UnityEngine;

[RequireComponent(typeof(CharacterHealth))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterDetection))]
public class CharacterManager : MonoBehaviour
{
    [Header("Components")]
    private CharacterHealth characterHealth;
    [SerializeField] private CircleCollider2D _col;
    

    private void Awake()
    {
        characterHealth = GetComponent<CharacterHealth>();  
    }


    public void TakeDamage(int _damage)
    {
        characterHealth.TakeDamage(_damage);    
    }

    public Vector2 GetColliderCenter()
    {
        return (Vector2)transform.position + _col.offset;
    }
}
