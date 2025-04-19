using Unity.VisualScripting;
using UnityEngine;

public class SpawnWeapon : MeleeWeapon
{
    [Header("SPAWN SETTINGS:")]
    [Tooltip("Prefab for the energy wave projectile.")]
    [SerializeField] private GameObject energyWavePrefab; 

    [Tooltip("Speed at which the energy wave travels.")]
    [SerializeField] private float waveSpeed = 5f; 

    [Tooltip("Amount of health restored by life steal.")]
    [SerializeField] private int lifeStealAmount = 10; 

    [SerializeField] private Transform spawnPoint;

    private bool canFire = true; 
    private Vector2 cachedMoveDirection;

   protected override void Attack()
    {
        Vector2 moveDirection = CharacterManager.Instance.controller.MoveDirection;

        if (canFire && moveDirection != Vector2.zero)
        {
            anim.Play("Attack"); 

            HandleWeaponDirection(moveDirection); 
            cachedMoveDirection = moveDirection; 
        }
    }

    public void TriggerWave()
    {
        if (canFire && cachedMoveDirection != Vector2.zero) 
            ReleaseEnergyWave(cachedMoveDirection); 
    }

    private void HandleWeaponDirection(Vector2 moveDirection)
    {
        if (moveDirection.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveDirection.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); 
        }
    }

    private void ReleaseEnergyWave(Vector2 moveDirection)
    {
        canFire = false; 

        GameObject energyWave = Instantiate(energyWavePrefab, spawnPoint.position, Quaternion.identity);
        EnergyWave wave = energyWave.GetComponent<EnergyWave>();
        wave.Initialize(damage, waveSpeed, enemyMask, lifeStealAmount, OnWaveComplete, moveDirection);
    }

    private void OnWaveComplete() =>  canFire = true; 
}
