using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ItemManager : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Candy candyPrefab;
    [SerializeField] private Cash cashPrefab;
    [SerializeField] private Chest chestPrefab;

    
    [Header("SETTINGS:")]
    [Range(0,100)]
    [SerializeField] private int cashDropChance;
    [Range(0,100)]
    [SerializeField] private int chestDropChance;

    [Header("Pooling")]
    private ObjectPool<Candy> candyPool;
    private ObjectPool<Cash> cashPool;
    private ObjectPool<Chest> chestPool;

    private void Awake()
    {
        Enemy.OnDeath += EnemyDeathCallback;
        Candy.OnCollected += ReleaseCandy;
        Cash.onCollected += ReleaseCash;    
        Chest.OnCollected += ReleaseChest;    
    }

    private void Start()
    {
        candyPool = new ObjectPool<Candy>(
            CandyCreateFunction, 
            CandyActionOnGet, 
            CandyActionOnRelease, 
            CandyActionOnDestroy);

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

    private void OnDestroy()
    {
        Enemy.OnDeath -= EnemyDeathCallback;
        Candy.OnCollected -= ReleaseCandy;
        Cash.onCollected -= ReleaseCash;    
        Chest.OnCollected -= ReleaseChest; 
    }

    private void EnemyDeathCallback(Vector2 _enemyPosition, int _enemyLevel)
    {
        bool shouldSpawnCash = Random.Range(0, 101) <= cashDropChance;

        Item itemToDrop = shouldSpawnCash ? cashPool.Get() : candyPool.Get();

        itemToDrop.transform.position = _enemyPosition;

        TryDropChest(_enemyPosition);
        
    }

    private void ReleaseCandy(Candy _candy) => candyPool.Release(_candy);    
    private void ReleaseCash(Cash _cash) => cashPool.Release(_cash);    
    private void ReleaseChest(Chest _chest) => chestPool.Release(_chest);    

    private void TryDropChest(Vector2 _spawnPosition)
    {
        bool shouldSpawnCash = Random.Range(0, 101) <= chestDropChance;

        if (!shouldSpawnCash)
           return;

        Item itemToDrop = chestPool.Get(); 

        itemToDrop.transform.position = _spawnPosition;
        
    }

    #region POOLING
    private Candy CandyCreateFunction() => Instantiate(candyPrefab, transform);
    private void CandyActionOnGet(Candy _candy) => _candy.gameObject.SetActive(true);
    private void CandyActionOnRelease(Candy _candy) =>  _candy.gameObject.SetActive(false);
    private void CandyActionOnDestroy(Candy _candy) => Destroy(_candy.gameObject);

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
