
using UnityEngine;
using UnityEngine.Pool;

public class DamageTextManager : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private DamageText damageTextPrefab;

    [Header("POOL:")]
    private ObjectPool<DamageText> damageTextPool;  

    private void Awake()
    {
        Enemy.OnDamageTaken += EnemyHitCallback;
        CharacterHealth.OnDodge += CharacterDodgeCallback;
    } 

    private void OnDestroy()
    {
        Enemy.OnDamageTaken -= EnemyHitCallback;
        CharacterHealth.OnDodge -= CharacterDodgeCallback;
    }
   
        
    private void Start() => damageTextPool = new ObjectPool<DamageText>(CreateFunction, ActionOnGet, ActionOnRelease, ActionOnDestroy);

    private DamageText CreateFunction() => Instantiate(damageTextPrefab, transform);

    private void ActionOnGet(DamageText _damageText) =>  _damageText.gameObject.SetActive(true);

    private void ActionOnRelease(DamageText _damageText)
    {
        if(_damageText.gameObject != null)
            _damageText.gameObject.SetActive(false);
    }

    private void ActionOnDestroy(DamageText _damageText) => Destroy(_damageText.gameObject);

    private void EnemyHitCallback(int _damage, Vector2 enemyPos, bool _isCriticalHit)
    {
        DamageText _damageText = damageTextPool.Get();

        Vector3 spawnPosition = enemyPos + Vector2.up * 1.5f;
        _damageText.transform.position = spawnPosition;

        _damageText.PlayAnimation(_damage.ToString(), _isCriticalHit);

        LeanTween.delayedCall(1, () => damageTextPool.Release(_damageText));
    }

    private void CharacterDodgeCallback(Vector2 _characterPosition)
    {   
        DamageText _damageText = damageTextPool.Get();

        Vector3 spawnPosition = _characterPosition + Vector2.up * 1.5f;
        _damageText.transform.position = spawnPosition;

        _damageText.PlayAnimation("Dodged", false);

        LeanTween.delayedCall(1, () => damageTextPool.Release(_damageText));
    }
}

