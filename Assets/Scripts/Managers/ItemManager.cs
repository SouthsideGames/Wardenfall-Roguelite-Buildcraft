
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class ItemManager : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Meat meatPrefab;
    [SerializeField] private Cash cashPrefab;
    [SerializeField] private Chest chestPrefab;
    [SerializeField] private Gem gemPrefab;

    
    [Header("SETTINGS:")]
    [Range(0,100)]
    [SerializeField] private int cashDropChance;
    [Range(0,100)]
    [SerializeField] private int chestDropChance;
    [Range(0,100)]
    [SerializeField] private int gemDropChance;

    [Header("Pooling")]
    private ObjectPool<Meat> meatPool;
    private ObjectPool<Cash> cashPool;
    private ObjectPool<Chest> chestPool;
    private ObjectPool<Gem> gemPool;

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

        TryDropChest(_enemyPosition);
        
    }

    private void BossDeathCallback(Vector2 _bossPosition) =>  DropChest(_bossPosition);

    private void ReleaseMeat(Meat _meat) => meatPool.Release(_meat);    
    private void ReleaseCash(Cash _cash) => cashPool.Release(_cash);    
    private void ReleaseChest(Chest _chest) => chestPool.Release(_chest);    
    private void ReleaseGem(Gem _gem) => gemPool.Release(_gem);    

    private void TryDropChest(Vector2 _spawnPosition)
    {
        bool shouldSpawnCash = Random.Range(0, 101) <= chestDropChance;

        if (!shouldSpawnCash)
           return;

        DropChest(_spawnPosition);
        
    }

    private void DropChest(Vector2 _spawnPosition) => Instantiate(chestPrefab, _spawnPosition, Quaternion.identity, transform);

    #region POOLING
    private Meat MeatCreateFunction() => Instantiate(meatPrefab, transform);
    private void MeatActionOnGet(Meat _meat) => _meat.gameObject.SetActive(true);
    private void MeatActionOnRelease(Meat _meat) =>  _meat.gameObject.SetActive(false);
    private void MeatActionOnDestroy(Meat _meat) => Destroy(_meat.gameObject);

    private Gem GemCreateFunction() => Instantiate(gemPrefab, transform);
    private void GemActionOnGet(Gem _gem) => _gem.gameObject.SetActive(true);
    private void GemActionOnRelease(Gem _gem) =>  _gem.gameObject.SetActive(false);
    private void GemActionOnDestroy(Gem _gem) => Destroy(_gem.gameObject);

    private Cash CashCreateFunction() => Instantiate(cashPrefab, transform);
    private void CashActionOnGet(Cash _cash) => _cash.gameObject.SetActive(true);
    private void CashActionOnRelease(Cash _cash) =>  _cash.gameObject.SetActive(false);
    private void CashActionOnDestroy(Cash _cash) => Destroy(_cash.gameObject);

    private Chest ChestCreateFunction() => Instantiate(chestPrefab, transform);
    private void ChestActionOnGet(Chest _chest) => _chest.gameObject.SetActive(true);
    private void ChestActionOnRelease(Chest _chest) =>  _chest.gameObject.SetActive(false);
    private void ChestActionOnDestroy(Chest _chest) => Destroy(_chest.gameObject);

    #endregion

    
}
