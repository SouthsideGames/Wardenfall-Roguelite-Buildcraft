
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

/// <summary>
/// Manages item drops including cash, chests, gems, and SurvivorBoxes. SurvivorBoxes
/// spawn either a CollectableWeapon or CollectableObject based on a timer or get destroyed if health reaches zero.
/// </summary>
public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [Header("ELEMENTS:")]
    [SerializeField] private Meat meatPrefab;
    [SerializeField] private Cash cashPrefab;
    [SerializeField] private Chest chestPrefab;

    [Header("SETTINGS:")]
    [Range(0, 100)]
    [SerializeField] private int baseCashDropChance;
    [Range(0, 100)]
    [SerializeField] private int baseChestDropChance;

    private float cashDropChanceMultiplier = 1f;
    private float chestDropChanceMultiplier = 1f;
    private float dropRateMultiplier = 1f;

    [Header("Pooling")]
    private ObjectPool<Meat> meatPool;
    private ObjectPool<Cash> cashPool;
    private ObjectPool<Chest> chestPool;

    private void Awake()
    {
         if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Enemy.OnDeath += EnemyDeathCallback;
        Enemy.OnBossDeath += BossDeathCallback;
        Meat.OnCollected += ReleaseMeat;
        Cash.OnCollected += ReleaseCash;
        Chest.OnCollected += ReleaseChest;
    }

    private void Start()
    {
        InitializePools();
    }

    private void OnDestroy()
    {
        Enemy.OnDeath -= EnemyDeathCallback;
        Enemy.OnBossDeath -= BossDeathCallback;
        Meat.OnCollected -= ReleaseMeat;
        Cash.OnCollected -= ReleaseCash;
        Chest.OnCollected -= ReleaseChest;
    }

    private void InitializePools()
    {
        meatPool = new ObjectPool<Meat>(
            MeatCreateFunction,
            MeatActionOnGet,
            MeatActionOnRelease,
            MeatActionOnDestroy);

        cashPool = new ObjectPool<Cash>(
            CashCreateFunction,
            CashActionOnGet,
            CashActionOnRelease,
            CashActionOnDestroy);

        chestPool = new ObjectPool<Chest>(
            ChestCreateFunction,
            ChestActionOnGet,
            ChestActionOnRelease,
            ChestActionOnDestroy);
    }

    public void ApplyItemBoost(float multiplier, float duration)
    {
        cashDropChanceMultiplier = multiplier;
        chestDropChanceMultiplier = multiplier;

        CancelInvoke(nameof(ResetItemBoost));
        Invoke(nameof(ResetItemBoost), duration);

    }

    private void ResetItemBoost()
    {
        cashDropChanceMultiplier = 1f;
        chestDropChanceMultiplier = 1f;

    }

    private void EnemyDeathCallback(Vector2 _enemyPosition)
    {
        int cashChance = Mathf.RoundToInt(baseCashDropChance * cashDropChanceMultiplier * dropRateMultiplier);

        Item itemToDrop = Random.Range(0f, 100f) < cashChance ? cashPool.Get() :
                          meatPool.Get();

        if (itemToDrop != null)
        {
            itemToDrop.transform.position = _enemyPosition;
        }

        if (CharacterManager.Instance.cards.HasCard("double_drop"))
        {
            float chance = 0.25f; // 25% chance
            if (Random.value < chance)
            {
                Item duplicate = Random.Range(0f, 100f) < cashChance ? cashPool.Get() : meatPool.Get();
                if (duplicate != null)
                {
                    duplicate.transform.position = _enemyPosition;
                }
            }
        }


        TryDropChest(_enemyPosition);
    }

    private void BossDeathCallback(Vector2 _bossPosition) => DropChest(_bossPosition);

    private void TryDropChest(Vector2 _spawnPosition)
    {
        float baseChance = baseChestDropChance * chestDropChanceMultiplier * dropRateMultiplier;
        float finalChance = baseChance + (ProgressionEffectManager.Instance != null ? ProgressionEffectManager.Instance.ChestDropBonus * 100f : 0f);
    
        bool shouldSpawnChest = Random.Range(0f, 100f) <= finalChance;
    
        if (!shouldSpawnChest)
            return;
    
        DropChest(_spawnPosition);
    }


    private void DropChest(Vector2 _spawnPosition) =>
        Instantiate(chestPrefab, _spawnPosition, Quaternion.identity, transform);

    private void ReleaseMeat(Meat _meat) => meatPool.Release(_meat);
    private void ReleaseCash(Cash _cash) => cashPool.Release(_cash);
    private void ReleaseChest(Chest _chest) => chestPool.Release(_chest);

    #region POOLING
    private Meat MeatCreateFunction() => Instantiate(meatPrefab, transform);
    private void MeatActionOnGet(Meat _meat) => _meat.gameObject.SetActive(true);
    private void MeatActionOnRelease(Meat _meat) => _meat.gameObject.SetActive(false);
    private void MeatActionOnDestroy(Meat _meat) => Destroy(_meat.gameObject);

    private Cash CashCreateFunction() => Instantiate(cashPrefab, transform);
    private void CashActionOnGet(Cash _cash) => _cash.gameObject.SetActive(true);
    private void CashActionOnRelease(Cash _cash) => _cash.gameObject.SetActive(false);
    private void CashActionOnDestroy(Cash _cash) => Destroy(_cash.gameObject);

    private Chest ChestCreateFunction() => Instantiate(chestPrefab, transform);
    private void ChestActionOnGet(Chest _chest) => _chest.gameObject.SetActive(true);
    private void ChestActionOnRelease(Chest _chest) => _chest.gameObject.SetActive(false);
    private void ChestActionOnDestroy(Chest _chest) => Destroy(_chest.gameObject);

    #endregion

    public void ModifyDropRates(float multiplier)
    {
        dropRateMultiplier = multiplier;
    }

}
