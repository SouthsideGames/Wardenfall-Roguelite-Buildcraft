
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
    [SerializeField] private Gem gemPrefab;

    [Header("SETTINGS:")]
    [Range(0, 100)]
    [SerializeField] private int baseCashDropChance;
    [Range(0, 100)]
    [SerializeField] private int baseChestDropChance;
    [Range(0, 100)]
    [SerializeField] private int baseGemDropChance;

    private float cashDropChanceMultiplier = 1f;
    private float chestDropChanceMultiplier = 1f;
    private float gemDropChanceMultiplier = 1f;
    private float dropRateMultiplier = 1f;

    [Header("Pooling")]
    private ObjectPool<Meat> meatPool;
    private ObjectPool<Cash> cashPool;
    private ObjectPool<Chest> chestPool;
    private ObjectPool<Gem> gemPool;

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
        Gem.OnCollected += ReleaseGem;
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
        Gem.OnCollected -= ReleaseGem;
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

        gemPool = new ObjectPool<Gem>(
            GemCreateFunction,
            GemActionOnGet,
            GemActionOnRelease,
            GemActionOnDestroy);
    }

    public void ApplyItemBoost(float multiplier, float duration)
    {
        cashDropChanceMultiplier = multiplier;
        chestDropChanceMultiplier = multiplier;
        gemDropChanceMultiplier = multiplier;

        CancelInvoke(nameof(ResetItemBoost));
        Invoke(nameof(ResetItemBoost), duration);

        Debug.Log($"Item drop chances boosted by {multiplier * 100}% for {duration} seconds.");
    }

    private void ResetItemBoost()
    {
        cashDropChanceMultiplier = 1f;
        chestDropChanceMultiplier = 1f;
        gemDropChanceMultiplier = 1f;

    }

    private void EnemyDeathCallback(Vector2 _enemyPosition)
    {
        int cashChance = Mathf.RoundToInt(baseCashDropChance * cashDropChanceMultiplier * dropRateMultiplier);
        int gemChance = Mathf.RoundToInt(baseGemDropChance * gemDropChanceMultiplier * dropRateMultiplier);


        Item itemToDrop = Random.Range(0f, 100f) < cashChance ? cashPool.Get() :
                          Random.Range(0f, 100f) < gemChance ? gemPool.Get() : meatPool.Get();

        if (itemToDrop != null)
        {
            itemToDrop.transform.position = _enemyPosition;
        }

        TryDropChest(_enemyPosition);
    }

    private void BossDeathCallback(Vector2 _bossPosition) => DropChest(_bossPosition);

    private void TryDropChest(Vector2 _spawnPosition)
    {
        int chestChance = Mathf.RoundToInt(baseChestDropChance * chestDropChanceMultiplier * dropRateMultiplier);

        bool shouldSpawnChest = Random.Range(0, 101) <= chestChance;

        if (!shouldSpawnChest)
            return;

        DropChest(_spawnPosition);
    }

    private void DropChest(Vector2 _spawnPosition) =>
        Instantiate(chestPrefab, _spawnPosition, Quaternion.identity, transform);

    private void ReleaseMeat(Meat _meat) => meatPool.Release(_meat);
    private void ReleaseCash(Cash _cash) => cashPool.Release(_cash);
    private void ReleaseChest(Chest _chest) => chestPool.Release(_chest);
    private void ReleaseGem(Gem _gem) => gemPool.Release(_gem);

    #region POOLING
    private Meat MeatCreateFunction() => Instantiate(meatPrefab, transform);
    private void MeatActionOnGet(Meat _meat) => _meat.gameObject.SetActive(true);
    private void MeatActionOnRelease(Meat _meat) => _meat.gameObject.SetActive(false);
    private void MeatActionOnDestroy(Meat _meat) => Destroy(_meat.gameObject);

    private Gem GemCreateFunction() => Instantiate(gemPrefab, transform);
    private void GemActionOnGet(Gem _gem) => _gem.gameObject.SetActive(true);
    private void GemActionOnRelease(Gem _gem) => _gem.gameObject.SetActive(false);
    private void GemActionOnDestroy(Gem _gem) => Destroy(_gem.gameObject);

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
