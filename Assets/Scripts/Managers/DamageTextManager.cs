using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class DamageTextManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private DamageText damageTextPrefab;

    [Header("Pooling")]
    private ObjectPool<DamageText> damageTextPool;  

    private void Awake() => MeleeEnemy.onDamageTaken += EnemyHitCallback;

    private void Start() => damageTextPool = new ObjectPool<DamageText>(CreateFunction, ActionOnGet, ActionOnRelease, ActionOnDestroy);

    private DamageText CreateFunction()
    {
        return Instantiate(damageTextPrefab, transform);
    }

    private void ActionOnGet(DamageText _damageText) =>  _damageText.gameObject.SetActive(true);

    private void ActionOnRelease(DamageText _damageText) =>  _damageText.gameObject.SetActive(false);
    private void ActionOnDestroy(DamageText _damageText) => Destroy(_damageText.gameObject);

    private void OnDestroy() => MeleeEnemy.onDamageTaken -= EnemyHitCallback;

    private void EnemyHitCallback(int _damage, Vector2 enemyPos, bool _isCriticalHit)
    {
        DamageText _damageText = damageTextPool.Get();

        Vector3 spawnPosition = enemyPos + Vector2.up * 1.5f;
        _damageText.transform.position = spawnPosition;

        _damageText.PlayAnimation(_damage, _isCriticalHit);

        LeanTween.delayedCall(1, () => damageTextPool.Release(_damageText));
    }
}

