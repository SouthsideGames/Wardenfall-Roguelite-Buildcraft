using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;
using Unity.Cinemachine;

[RequireComponent(typeof(RangedEnemyAttack))]
public class Boss : Enemy
{
    [Header("ADD. ELEMENTS:")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Animator anim;

    private BossState bossState;
    
    [Header("IDLE STATE:")]
    [SerializeField] private float maxIdleDuration = 0.1f;
    private float idleDuration;
    private float idleTimer;
    
    [Header("MOVING STATE:")]
    [SerializeField] private float moveSpeed;
    private Vector2 targetPosition;

    [Header("ATTACK STATE:")]
    private int attackCounter;
    private RangedEnemyAttack rangedEnemyAttack;

    private void Awake() 
    {
        bossState = BossState.None;

        healthBar.gameObject.SetActive(false);

        OnSpawnCompleted += SpawnCompletedCallback;
        OnDamageTaken += DamageTakenCallback;
    }

    private void OnDestroy() 
    {
        OnSpawnCompleted -= SpawnCompletedCallback;
        OnDamageTaken -= DamageTakenCallback;   
    }

    protected override void Start()
    {
        base.Start();
        rangedEnemyAttack = GetComponent<RangedEnemyAttack>();
    }

    protected override void Update() 
    {
        ManageStates();
    }

    private void ManageStates()
    {
        switch(bossState)
        {
            case BossState.Idle:
                ManageIdleState();
                break;

            case BossState.Moving:
                ManageMovingState();
                break;

            case BossState.Attacking:
                ManageAttackingState();
                break;

            default:
                break;
        }
    }

    private void SetIdleState()
    {
        bossState = BossState.Idle;

        idleDuration = Random.Range(1f, maxIdleDuration);

        anim.Play("Idle");
    }


    private void ManageIdleState()
    {
        idleTimer += Time.deltaTime;  

        if(idleTimer >= idleDuration)
        {
            idleTimer = 0;
            StartMovingState();
        }
    }

    private void StartMovingState()
    {
        bossState = BossState.Moving;

        targetPosition = GetRandomPosition();
        anim.Play("Move");
    }


    private void ManageMovingState()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if(Vector2.Distance(transform.position, targetPosition) < .01f)
            StartAttackingState();
    }

    private void StartAttackingState()
    {
        bossState = BossState.Attacking;
        attackCounter = 0;
        anim.Play("Attack");
    }

    private void ManageAttackingState()
    {

    }



    protected override void Attack()
    {
        Vector2 direction = Quaternion.Euler(0, 0, -45 * attackCounter) * Vector2.up;
        rangedEnemyAttack.InstantShoot(direction);
        attackCounter++;

    }

    private void UpdateHealthUI()
    {
        healthBar.value = (float)health / maxHealth;
        healthText.text = $"{health} / {maxHealth}";
    }

    private void SpawnCompletedCallback()
    {
        UpdateHealthUI();
        healthBar.gameObject.SetActive(true);
        
        SetIdleState();
    }

    private Vector2 GetRandomPosition()
    {
        Vector2 targetPosition = Vector2.zero;

        targetPosition.x = Mathf.Clamp(targetPosition.x, -14, 14);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -6, 6);
        
        return targetPosition;
    }

    private void DamageTakenCallback(int _damage, Vector2 _position, bool _isCriticalHit) => UpdateHealthUI();

}
