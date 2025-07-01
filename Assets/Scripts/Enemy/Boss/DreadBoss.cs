using System.Collections;
using UnityEngine;

public class DreadBoss : Boss
{
    [Header("STAGE 1 SETTINGS")]
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private float spikeFallRange = 5f;
    [SerializeField] private int numSpikes = 6;

    [Header("STAGE 2 SETTINGS")]
    [SerializeField] private GameObject webPrefab;
    [SerializeField] private int numWebs = 3;
    [SerializeField] private float webDuration = 5f;

    private bool isAttacking;
    private Vector3 originalScale;
    private EnemyMovement enemyMovement;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        originalScale = transform.localScale;
        enemyMovement = GetComponent<EnemyMovement>();
        attackTimer = attackCooldown;
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            ExecuteStage();
            attackTimer = attackCooldown;
        }
        else
            enemyMovement.FollowCurrentTarget();
    }

    protected override void ExecuteStage()
    {
        if (GetHealthPercent() > 0.5f)
            StartCoroutine(PerformGroundSlam());
        else
            StartCoroutine(PerformWebTrap());
    }

    private float GetHealthPercent()
    {
        return (float)health / maxHealth;
    }

    private IEnumerator PerformGroundSlam()
    {
        isAttacking = true;
        enemyMovement.DisableMovement(1.5f);

        LeanTween.color(gameObject, Color.red, 0.2f).setLoopPingPong(2);
        LeanTween.scale(gameObject, originalScale * 1.2f, 0.2f).setEaseInOutQuad();

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < numSpikes; i++)
        {
            Vector2 spawnPosition = GetRandomTopScreenPosition();
            Instantiate(spikePrefab, spawnPosition, Quaternion.identity);
        }

        LeanTween.scale(gameObject, originalScale * 1.05f, 0.1f).setEasePunch();
        yield return new WaitForSeconds(0.5f);

        LeanTween.scale(gameObject, originalScale, 0.1f);
        isAttacking = false;
    }

    private IEnumerator PerformWebTrap()
    {
        isAttacking = true;
        enemyMovement.DisableMovement(2f);

        LeanTween.color(gameObject, Color.green, 0.2f).setLoopPingPong(2);
        LeanTween.scale(gameObject, originalScale * 1.2f, 0.2f).setEaseInOutQuad();

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < numWebs; i++)
        {
            Vector2 webPosition = new Vector2(
                Random.Range(-spikeFallRange, spikeFallRange) + transform.position.x,
                Random.Range(-spikeFallRange, spikeFallRange) + transform.position.y
            );

            GameObject web = Instantiate(webPrefab, webPosition, Quaternion.identity);
            Destroy(web, webDuration);
        }

        LeanTween.scale(gameObject, originalScale * 1.05f, 0.1f).setEasePunch();
        yield return new WaitForSeconds(0.5f);

        LeanTween.scale(gameObject, originalScale, 0.1f);
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
