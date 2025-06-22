using System.Collections;
using UnityEngine;

public class ShieldEnemy : Enemy
{
    [Header("SHIELD SPECIFIC:")]
    [SerializeField] private int shieldHealth = 50;
    [SerializeField] private GameObject shield;

    [Header("SHIELD HIT ANIMATION")]
    [SerializeField] private float hitScaleFactor = 1.2f;
    [SerializeField] private float hitAnimDuration = 0.1f;

    [Header("SHIELD BREAK ANIMATION")]
    [SerializeField] private float breakAnimDuration = 0.2f;

    [Header("SHIELD PULSE")]
    [SerializeField] private float pulseScaleFactor = 1.05f;
    [SerializeField] private float pulseSpeed = 0.8f;

    private LTDescr pulseTween;

    protected override void Start()
    {
        base.Start();

        if (movement != null)
        {
            movement.patrol = true;
            movement.chasePlayer = false;
        }

        if (shield != null) shield.SetActive(false);
        OnSpawnCompleted += ActivateShieldAfterSpawn;
    }

    private void ActivateShieldAfterSpawn()
    {
        if (shield == null) return;

        shield.SetActive(true);
        shield.transform.localScale = Vector3.one;
        StartShieldPulse();
    }

    public override void TakeDamage(int _damage, bool _isCriticalHit)
    {
        if (shieldHealth > 0)
        {
            shieldHealth -= _damage;
            PlayShieldHitAnimation();

            if (shieldHealth < 0)
            {
                PlayShieldBreakAnimation();
                health += shieldHealth;
                shieldHealth = 0;
            }
        }
        else
        {
            base.TakeDamage(_damage, _isCriticalHit);
        }
    }

    private void PlayShieldHitAnimation()
    {
        if (shield == null || !shield.activeInHierarchy) return;

        LeanTween.cancel(shield);
        LeanTween.scale(shield, Vector3.one * hitScaleFactor, hitAnimDuration)
            .setEasePunch()
            .setOnComplete(() => shield.transform.localScale = Vector3.one);
    }

    private void PlayShieldBreakAnimation()
    {
        if (shield == null || !shield.activeInHierarchy) return;

        LeanTween.cancel(shield);
        LeanTween.scale(shield, Vector3.zero, breakAnimDuration)
            .setEaseInBack()
            .setOnComplete(() => shield.SetActive(false));
    }

    private void StartShieldPulse()
    {
        if (shield == null) return;

        LeanTween.cancel(shield);
        pulseTween = LeanTween.scale(shield, Vector3.one * pulseScaleFactor, pulseSpeed)
            .setEaseInOutSine()
            .setLoopPingPong();
    }

    protected override void Update()
    {
        base.Update();
        movement?.FollowCurrentTarget();
    }
}
