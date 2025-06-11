using UnityEngine;
using System.Collections;

public class CharacterAbility : MonoBehaviour
{
    private CharacterController controller;
    public AbilityType currentAbility;

    private float cooldownTime;
    private float cooldownRemaining;

    private float dashCooldownModifier = 1f;

    void Awake()
    {
        controller = CharacterManager.Instance.controller;

    }

    private void Update()
    {
        if (cooldownRemaining > 0)
            cooldownRemaining -= Time.deltaTime;
    }

    public void ApplyDashCooldownModifier(float modifier)
    {
        dashCooldownModifier = modifier;
    }


    public void TryUseAbility()
    {
        if (cooldownRemaining > 0) return;

        switch (currentAbility)
        {
            case AbilityType.FlameJet:
                UseFlameJet(); cooldownTime = 7f; break;
            case AbilityType.LuckyRoll:
                UseLuckyRoll(); cooldownTime = 8f; break;
            case AbilityType.TargetLockDash:
                UseTargetLockDash(); cooldownTime = 6f; break;
            case AbilityType.SlipstreamSurge:
                UseSlipstreamSurge(); cooldownTime = 5f; break;
            case AbilityType.BeastLunge:
                UseBeastLunge(); cooldownTime = 9f; break;
            case AbilityType.QuantumBlink:
                UseQuantumBlink(); cooldownTime = 10f; break;
            case AbilityType.UppercutDash:
                UseUppercutDash(); cooldownTime = 6f; break;
            case AbilityType.Shadowstep:
                UseShadowstep(); cooldownTime = 8f; break;
            case AbilityType.GhostGlide:
                UseGhostGlide(); cooldownTime = 7f; break;
            case AbilityType.SoulFlicker:
                UseSoulFlicker(); cooldownTime = 6f; break;
        }

        cooldownRemaining = cooldownTime * dashCooldownModifier;
    }

    public float GetCooldownTime() => cooldownTime;

    private Vector2 GetInputDirectionOrFallback()
    {
        Vector2 dir = controller.LastInputDirection;
        return dir == Vector2.zero ? Vector2.down : dir.normalized;
    }

    private void Heal(int amount) => CharacterManager.Instance.health.Heal(amount);

    private IEnumerator ApplyTemporaryDodge(float duration)
    {

        CharacterManager.Instance.stats.BoostStat(Stat.Dodge, 30f);
        yield return new WaitForSeconds(duration);
        CharacterManager.Instance.stats.RevertBoost(Stat.Dodge);
    }

    private IEnumerator ApplyTemporaryInvisibility(float duration)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1, 1, 1, 0.3f);
        gameObject.layer = LayerMask.NameToLayer("IgnoreEnemies");
        yield return new WaitForSeconds(duration);
        sr.color = Color.white;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void UseFlameJet() => controller.TriggerDash(GetInputDirectionOrFallback(), 20f, 0.2f);
    private void UseLuckyRoll()
    {
        controller.TriggerDash(GetInputDirectionOrFallback(), 15f, 0.25f);
        StartCoroutine(ApplyTemporaryDodge(1f));
    }
    private void UseTargetLockDash() => controller.TriggerDash(GetInputDirectionOrFallback(), 18f, 0.2f);
    private void UseSlipstreamSurge()
    {
        controller.TriggerDash(GetInputDirectionOrFallback(), 22f, 0.15f);
        Heal(10);
    }
    private void UseBeastLunge()
    {
        Vector2 dashDir = GetInputDirectionOrFallback();
        controller.TriggerDash(dashDir, 17f, 0.25f);
        StartCoroutine(ApplyDeathMarkAfterDash(0.25f));
    }


    private void UseQuantumBlink()
    {
        Vector2 offset = GetInputDirectionOrFallback() * 5f;
        Vector2 oldPos = transform.position;
        transform.position += (Vector3)offset;
        // TODO: Spawn turret at oldPos
    }
    private void UseUppercutDash() => controller.TriggerDash(GetInputDirectionOrFallback(), 18f, 0.2f);
    private void UseShadowstep()
    {
        Vector2 blink = GetInputDirectionOrFallback() * 5f;
        transform.position += (Vector3)blink;
        StartCoroutine(ApplyTemporaryInvisibility(2f));
    }
    private void UseGhostGlide() => controller.TriggerDash(GetInputDirectionOrFallback(), 16f, 0.25f);
    private void UseSoulFlicker() => controller.TriggerDash(GetInputDirectionOrFallback(), 19f, 0.2f);
    
    private IEnumerator ApplyDeathMarkAfterDash(float delay)
    {
        yield return new WaitForSeconds(delay);

        float radius = 2.5f;
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            EnemyStatus statusHandler = hit.GetComponent<EnemyStatus>();
            if (statusHandler != null)
            {
                StatusEffect deathMarkEffect = new StatusEffect(StatusEffectType.DeathMark, 5f, 1.5f, 0f);
                statusHandler.ApplyEffect(deathMarkEffect);
            }
        }
    }

}
