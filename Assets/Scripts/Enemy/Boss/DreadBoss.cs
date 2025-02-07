using System.Collections;
using UnityEngine;

public class DreadBoss : Boss
{
   [Header("STAGE 1")]
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private GameObject webPrefab;
    [SerializeField] private float spikeFallRange = 5f;
    [SerializeField] private int numSpikes = 6;

    [Header("STAGE 2")]
    [SerializeField] private int numWebs = 3;
    [SerializeField] private float webDuration = 5f;

    private bool isAttacking;
    private Vector3 originalScale;
    
    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        originalScale = transform.localScale;
    }

    protected override void ExecuteStageOne()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            GroundSlam();
        }
    }

    protected override void ExecuteStageTwo()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            WebTrap();
        }
    }

    private void GroundSlam()
    {
        // **1. Play Slam Animation**
        anim.Play("Slam");
        transform.localScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);

        // **2. Spawn EXACTLY numSpikes at the top of the screen**
        Debug.Log($"Dreadfang Ground Slam - Spawning {numSpikes} Spikes");

        for (int i = 0; i < numSpikes; i++)
        {
            Vector2 spawnPosition = GetRandomTopScreenPosition();
            Instantiate(spikePrefab, spawnPosition, Quaternion.identity);
        }

        // **3. Reset size and continue**
        transform.localScale = originalScale;
        isAttacking = false;
    }

    private void WebTrap()
    {
        // **1. Play Summoning Web Animation**
        anim.Play("SummonWebs");

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

        isAttacking = false;
    }

    private Vector2 GetRandomTopScreenPosition()
    {
        Camera mainCamera = Camera.main;
        float screenTopY = mainCamera.ViewportToWorldPoint(new Vector2(0.5f, 1f)).y + 1f;
        float randomX = Random.Range(
            mainCamera.ViewportToWorldPoint(new Vector2(0.1f, 0f)).x,
            mainCamera.ViewportToWorldPoint(new Vector2(0.9f, 0f)).x
        );

        return new Vector2(randomX, screenTopY);
    }
}
