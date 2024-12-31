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

    private bool canFire = true; // Controls firing state
    private Vector2 cachedMoveDirection;

   protected override void Attack()
    {
        Vector2 moveDirection = CharacterManager.Instance.controller.MoveDirection;

        // Only start attack if the player is moving
        if (canFire && moveDirection != Vector2.zero)
        {
            anim.Play("Attack"); // Play attack animation

            HandleWeaponDirection(moveDirection); // Adjust weapon orientation
            cachedMoveDirection = moveDirection; // Store movement direction
        }
    }

    // Animation Event Trigger: Call this from the animation
    public void TriggerWave()
    {
        if (canFire && cachedMoveDirection != Vector2.zero) // Ensure movement direction is valid
        {
            ReleaseEnergyWave(cachedMoveDirection); // Use cached direction for wave
        }
    }

    private void HandleWeaponDirection(Vector2 moveDirection)
    {
        // Flip weapon based on movement direction (left or right)
        if (moveDirection.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Flip horizontally
        }
        else if (moveDirection.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Reset flip
        }
    }

    private void ReleaseEnergyWave(Vector2 moveDirection)
    {
        canFire = false; // Block firing until the wave is complete

        // Instantiate energy wave
        GameObject energyWave = Instantiate(energyWavePrefab, spawnPoint.position, Quaternion.identity);
        EnergyWave wave = energyWave.GetComponent<EnergyWave>();
        wave.Initialize(damage, waveSpeed, enemyMask, lifeStealAmount, OnWaveComplete, moveDirection); // Pass direction
    }

    private void OnWaveComplete()
    {
        canFire = true; // Allow the next wave to fire
    }
}
