using UnityEngine;

public class LifeDrainWeapon : MeleeWeapon
{
    [Header("DRAIN SETTINGS:")]
    [SerializeField] private float drainDuration = 5.0f; 

    protected override void AttackLogic()
    {
        base.AttackLogic();

        if (closestEnemy != null)
        {
            closestEnemy.ApplyLifeDrain(drainDuration);
        }
    }
}
