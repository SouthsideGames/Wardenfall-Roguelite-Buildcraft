using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Add Shield Feedback
public class ShieldEnemy : MeleeEnemy
{
    [Header("SHIELD SPECIFIC:")]
    [SerializeField] private int shieldHealth = 50;
    [SerializeField] private GameObject shield;


    protected override void Start()
    {
        base.Start();

        shield.SetActive(true); 

    }

    public override void TakeDamage(int _damage, bool _isCriticalHit)
    {
        if (shieldHealth > 0)
        {
            shieldHealth -= _damage;
            
            if (shieldHealth < 0)
            {
                PlayShieldBreakParticles();
                health += shieldHealth;  // Transfer any excess damage to actual health
                shieldHealth = 0;
            }
        }
        else
        {
            base.TakeDamage(_damage, _isCriticalHit);
        }
    }

    private void PlayShieldBreakParticles() => shield.SetActive(false);
}
