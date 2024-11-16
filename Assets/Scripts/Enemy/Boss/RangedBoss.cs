using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;  

public class Boss : Enemy
{
    [Header("ADD. ELEMENTS:")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    private BossState bossState;
    
    [Header("IDLE STATE:")]
    [SerializeField] private float maxIdleDuration = 0.1f;
    private float idleDuration;
    private float idleTimer;
    
    [Header("MOVING STATE:")]
    [SerializeField] private float moveSpeed;

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

    protected override void Start() => base.Start();

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
    }

    private void ManageMovingState()
    {

    }

    private void ManageAttackingState()
    {

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


    private void DamageTakenCallback(int _damage, Vector2 _position, bool _isCriticalHit) => UpdateHealthUI();

}
