
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
   [Header("ELEMENTS:")]
    [SerializeField] private Meat meatPrefab;
    [SerializeField] private Cash cashPrefab;
    [SerializeField] private Chest chestPrefab;
    [SerializeField] private Gem gemPrefab;
    [SerializeField] private SurvivorBox survivorBoxPrefab;

    [Header("SETTINGS:")]
    [Range(0, 100)]
    [SerializeField] private int cashDropChance;
    [Range(0, 100)]
    [SerializeField] private int chestDropChance;
    [Range(0, 100)]
    [SerializeField] private int gemDropChance;
    [Range(0, 100)]
    [SerializeField] private int survivorBoxDropChance;

    [Header("Pooling")]
    private ObjectPool<Meat> meatPool;
    private ObjectPool<Cash> cashPool;
    private ObjectPool<Chest> chestPool;
    private ObjectPool<Gem> gemPool;
    private ObjectPool<SurvivorBox> survivorBoxPool;

    private void Awake()
    {
        Enemy.OnDeath += EnemyDeathCallback;
        Enemy.OnBossDeath += BossDeathCallback;
        Meat.OnCollected += ReleaseMeat;
        Cash.onCollected += ReleaseCash;
        Chest.OnCollected += ReleaseChest;
        Gem.OnCollected += ReleaseGem;
    }

    private void Start()
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

        survivorBoxPool = new ObjectPool<SurvivorBox>(
            SurvivorBoxCreateFunction,
            SurvivorBoxActionOnGet,
            SurvivorBoxActionOnRelease,
            SurvivorBoxActionOnDestroy);
    }

    private void OnDestroy()
    {
        Enemy.OnDeath -= EnemyDeathCallback;
        Enemy.OnBossDeath -= BossDeathCallback;
        Meat.OnCollected -= ReleaseMeat;
        Cash.onCollected -= ReleaseCash;
        Chest.OnCollected -= ReleaseChest;
        Gem.OnCollected -= ReleaseGem;
    }

    private void EnemyDeathCallback(Vector2 _enemyPosition)
    {
        Item itemToDrop = Random.Range(0f, 100f) < cashDropChance ? cashPool.Get() :
                          Random.Range(0f, 100f) < gemDropChance ? gemPool.Get() : meatPool.Get();

        if (itemToDrop != null)
        {
            itemToDrop.transform.position = _enemyPosition;
        }

        TryDropSurvivorBox(_enemyPosition);
        TryDropChest(_enemyPosition);
    }

    private void BossDeathCallback(Vector2 _bossPosition)
    {
        DropChest(_bossPosition);
        TryDropSurvivorBox(_bossPosition);
    }

    private void TryDropSurvivorBox(Vector2 spawnPosition)
    {
        if (GameModeManager.Instance.CurrentGameMode == GameMode.Survival)
        {
            bool shouldSpawnBox = Random.Range(0, 101) <= survivorBoxDropChance;

            if (shouldSpawnBox)
            {
                SurvivorBox box = survivorBoxPool.Get();
                box.transform.position = spawnPosition;
                box.Activate(); // Assume this method starts the box's timer and initializes its health.
            }
        }
    }

    private void TryDropChest(Vector2 _spawnPosition)
    {
        bool shouldSpawnChest = Random.Range(0, 101) <= chestDropChance;

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

    private SurvivorBox SurvivorBoxCreateFunction() => Instantiate(survivorBoxPrefab, transform);
    private void SurvivorBoxActionOnGet(SurvivorBox _box) => _box.gameObject.SetActive(true);
    private void SurvivorBoxActionOnRelease(SurvivorBox _box) => _box.gameObject.SetActive(false);
    private void SurvivorBoxActionOnDestroy(SurvivorBox _box) => Destroy(_box.gameObject);
    #endregion
}
