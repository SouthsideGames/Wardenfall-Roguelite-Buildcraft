using System.Collections;
using UnityEngine;

public class DreadBoss : Boss
{
   [Header("Dread Settings")]
    [SerializeField] private float slamCooldown = 5f;
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private GameObject webPrefab;
    [SerializeField] private float spikeFallRange = 5f;
    [SerializeField] private int numSpikes = 6;

    [Header("Web Trap Settings")]
    [SerializeField] private int numWebs = 3;
    [SerializeField] private float webDuration = 5f;
    [SerializeField] private float webEffectDuration = 2f;

    private int currentStage = 1;
    private bool isAttacking;
    private Vector3 originalScale;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        originalScale = transform.localScale;
        StartCoroutine(AttackLoop());
    }

    protected override void ManageStates()
    {
        switch (bossState)
        {
            case BossState.Idle:
                ManageIdleState();
                break;

            case BossState.Moving:
                ManageMovingState();
                break;

            case BossState.Attacking:
                ExecuteAttack();
                break;

            case BossState.Transitioning:
                ManageTransition();
                break;
        }
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(slamCooldown);
            
            if (!isAttacking && bossState == BossState.Idle) // PREVENT MULTIPLE SLAMS
            {
                StartAttackingState();
            }
        }
    }

    private IEnumerator GroundSlam()
    {
        isAttacking = true;

        // **1. Play Slam Animation**
        anim.Play("Slam");
        transform.localScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);
        yield return new WaitForSeconds(0.5f);

        // **2. Spawn EXACTLY numSpikes at the top of the screen**
        Debug.Log($"Dreadfang Ground Slam - Spawning {numSpikes} Spikes");

        for (int i = 0; i < numSpikes; i++) // ENSURES ONLY numSpikes SPAWN
        {
            Vector2 spawnPosition = GetRandomTopScreenPosition();
            Instantiate(spikePrefab, spawnPosition, Quaternion.identity);
            Debug.Log($"Spike {i + 1} spawned at {spawnPosition}");
        }

        yield return new WaitForSeconds(0.5f);

        // **3. Reset size and continue**
        transform.localScale = originalScale;
        isAttacking = false;
        SetIdleState();
    }

    private Vector2 GetRandomTopScreenPosition()
    {
        Camera mainCamera = Camera.main;
        float screenTopY = mainCamera.ViewportToWorldPoint(new Vector2(0.5f, 1f)).y + 1f; // Adjusted to ensure off-screen spawn
        float randomX = Random.Range(
            mainCamera.ViewportToWorldPoint(new Vector2(0.1f, 0f)).x, // Prevents spawning off-screen
            mainCamera.ViewportToWorldPoint(new Vector2(0.9f, 0f)).x
        );

        return new Vector2(randomX, screenTopY);
    }

    private IEnumerator WebTrap()
    {
        isAttacking = true;

        // **1. Animation for summoning webs**
        anim.Play("SummonWebs");
        yield return new WaitForSeconds(0.5f);

        // **2. Spawn Webs**
        for (int i = 0; i < numWebs; i++)
        {
            Vector2 webPosition = new Vector2(
                Random.Range(-spikeFallRange, spikeFallRange) + transform.position.x,
                Random.Range(-spikeFallRange, spikeFallRange) + transform.position.y
            );

            GameObject web = Instantiate(webPrefab, webPosition, Quaternion.identity);
            Destroy(web, webDuration);
        }

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        SetIdleState();
    }

    protected override void ExecuteAttack()
    {
        isAttacking = true;

        if (currentStage == 1)
            StartCoroutine(GroundSlam());
        else if (currentStage == 2)
            StartCoroutine(WebTrap());
    }

    private IEnumerator TransitionToStage2()
    {
        bossState = BossState.Transitioning;

        // **1. Grow in size to signal phase change**
        transform.localScale *= 1.2f;
        yield return new WaitForSeconds(1f);

        currentStage = 2;
        Debug.Log("Dreadfang has evolved to Stage 2!");

        transform.localScale = originalScale * 1.2f;
        movement.EnableMovement();
        SetIdleState();
    }

    private void ManageTransition()
    {
        StartCoroutine(TransitionToStage2());
    }
}
