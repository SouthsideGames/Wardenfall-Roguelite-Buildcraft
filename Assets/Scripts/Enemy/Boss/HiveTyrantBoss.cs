using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HiveTyrantBoss : Boss
{   
    
    [Header("DASH SETTINGS")]
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private float dashDistance = 3f;

    [Header("SUMMON SETTINGS")]
    [SerializeField] private GameObject stingletPrefab;
    [SerializeField] private int maxMinions = 5;

    private int activeMinions = 0;
    private EnemyMovement enemyMovement;
    private Vector3 originalScale;
    private bool isDashing;
    private float dashTimer;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        originalScale = transform.localScale;

        dashTimer = dashCooldown;
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isDashing) return;

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            ExecuteStage();
            dashTimer = dashCooldown;
        }
    }

    protected override void ExecuteStageOne()
    {
        if (GetHealthPercent() > 0.5f)
            StartCoroutine(PerformDash());
        else
            StartCoroutine(SpawnMinionPhase());
    }

    protected override void ExecuteStageTwo()
    {
        StartCoroutine(SpawnMinionPhase());
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        enemyMovement.DisableMovement(0.6f);

        LeanTween.color(gameObject, Color.red, 0.2f).setLoopPingPong(2);
        LeanTween.scale(gameObject, new Vector3(originalScale.x * 1.5f, originalScale.y * 0.7f, originalScale.z), 0.2f).setEaseInOutQuad();

        yield return new WaitForSeconds(0.2f);

        Vector2 dashDirection = (PlayerTransform.position - transform.position).normalized;
        Vector2 dashTarget = (Vector2)transform.position + dashDirection * dashDistance;

        enemyMovement.SetTargetPosition(dashTarget);
        Invoke(nameof(ResetAfterDash), 0.5f);
    }

    private void ResetAfterDash()
    {
        transform.localScale = originalScale;
        isDashing = false;
    }

    private IEnumerator SpawnMinionPhase()
    {
        enemyMovement.canMove = false;

        if (activeMinions >= maxMinions)
        {
            yield return new WaitForSeconds(1f);
            enemyMovement.canMove = true;
            yield break;
        }

        LeanTween.scale(gameObject, transform.localScale * 1.2f, 0.2f).setEasePunch();
        LeanTween.color(gameObject, Color.yellow, 0.2f).setLoopPingPong(2);

        yield return new WaitForSeconds(0.3f);

        Instantiate(stingletPrefab, transform.position, Quaternion.identity);
        activeMinions++;

        yield return new WaitForSeconds(1f);
        enemyMovement.canMove = true;
    }

    private float GetHealthPercent()
    {
        return (float)health / maxHealth;
    }

}
