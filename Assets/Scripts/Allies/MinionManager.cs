using System.Collections;
using UnityEngine;

public class MinionManager : MonoBehaviour
{
    [Header("SETTINGS:")]
    [SerializeField] private SpriteRenderer characterSprite;
    private float lifetime;
    private int damage;

    private void Start() => StartCoroutine(LifetimeCountdown());
    
    public void InitializeMinion(float _lifetime, int _damage)
    {
        characterSprite.sprite = CharacterStats.Instance.CharacterData.Icon;
        lifetime = _lifetime;
        damage = _damage;
    }

    public int GetDamage() => damage;

    private IEnumerator LifetimeCountdown()
    {
        yield return new WaitForSeconds(lifetime);
        DestroyMinion();
    }

    private void DestroyMinion() => Destroy(gameObject);
}
