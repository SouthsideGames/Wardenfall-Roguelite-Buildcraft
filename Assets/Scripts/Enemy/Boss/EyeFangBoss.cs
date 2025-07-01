using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RangedEnemyAttack))]
public class EyeFangBoss : Boss
{
    [Header("STAGE 1 SETTINGS")]
    [SerializeField] private float erraticMoveSpeed = 3f;

    [Header("STAGE 2 SETTINGS")]
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private int beamCount = 8;

    [Header("STAGE 3 SETTINGS")]
    [SerializeField] private float burstRadius = 3f;
    [SerializeField] private int burstDamage = 20;

    private RangedEnemyAttack rangedAttack;
    private EnemyMovement enemyMovement;
    private Vector3 originalScale;
    private bool isAttacking;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        rangedAttack = GetComponent<RangedEnemyAttack>();
        enemyMovement = GetComponent<EnemyMovement>();
        originalScale = transform.localScale;
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        if (GetHealthPercent() > 0.66f)
            enemyMovement.FollowCurrentTarget();
        else if (GetHealthPercent() > 0.33f)
            MoveErratically();
        else
            enemyMovement.StopMoving();
    }

    protected override void ExecuteStage()
    {
        if (GetHealthPercent() > 0.66f)
            StartCoroutine(FireProjectileAttack());
        else if (GetHealthPercent() > 0.33f)
            StartCoroutine(RadialBeamAttack());
        else
            StartCoroutine(KnockbackBurst());
    }

    private float GetHealthPercent()
    {
        return (float)health / maxHealth;
    }

    private void MoveErratically()
    {
        transform.position += new Vector3(
            Mathf.Sin(Time.time * 3),
            Mathf.Cos(Time.time * 3),
            0) * Time.deltaTime * erraticMoveSpeed;
    }

    private IEnumerator FireProjectileAttack()
    {
        isAttacking = true;
        enemyMovement.DisableMovement(1.5f);

        LeanTween.color(gameObject, Color.yellow, 0.2f).setLoopPingPong(2);
        LeanTween.scale(gameObject, originalScale * 1.1f, 0.2f).setEasePunch();

        yield return new WaitForSeconds(0.3f);

        rangedAttack.AutoAim();

        LeanTween.scale(gameObject, originalScale, 0.1f).setEaseOutQuad();
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }

    private IEnumerator RadialBeamAttack()
    {
        isAttacking = true;
        enemyMovement.DisableMovement(2f);

        LeanTween.color(gameObject, Color.cyan, 0.2f).setLoopPingPong(2);
        LeanTween.scale(gameObject, originalScale * 1.2f, 0.2f).setEaseInOutQuad();

        yield return new WaitForSeconds(0.4f);

        float angleStep = 360f / beamCount;
        for (int i = 0; i < beamCount; i++)
        {
            float angle = i * angleStep;
            Instantiate(beamPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        }

        LeanTween.scale(gameObject, originalScale * 1.05f, 0.1f).setEasePunch();
        yield return new WaitForSeconds(0.5f);

        LeanTween.scale(gameObject, originalScale, 0.1f);
        isAttacking = false;
    }

    private IEnumerator KnockbackBurst()
    {
        isAttacking = true;
        enemyMovement.DisableMovement(2f);

        LeanTween.color(gameObject, Color.red, 0.2f).setLoopPingPong(3);
        LeanTween.scale(gameObject, originalScale * 1.3f, 0.2f).setEaseInOutQuad();

        yield return new WaitForSeconds(0.5f);

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, burstRadius);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
                character.TakeDamage(burstDamage);
        }

        LeanTween.scale(gameObject, originalScale * 1.05f, 0.1f).setEasePunch();
        yield return new WaitForSeconds(0.5f);

        LeanTween.scale(gameObject, originalScale, 0.1f);
        isAttacking = false;
    }
}