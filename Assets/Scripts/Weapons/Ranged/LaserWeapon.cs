using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : RangedWeapon
{
    [Header("LASER SPECIFICS:")]
    [SerializeField] private LineRenderer laserBeam;

    void Update()
    {
        AutoAim();
        Shoot();
    }

    protected override void Shoot()
    {
        laserBeam.SetPosition(0, transform.position);
        laserBeam.SetPosition(1, transform.position + (Vector3)targetUpVector * range);

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, targetUpVector, range, enemyMask);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy)
                enemy.TakeDamage((int)(damage * Time.deltaTime), false);
        }
    }


    protected override void AutoAim()
    {
        base.AutoAim();
        if (closestEnemy != null)
            targetUpVector = (closestEnemy.transform.position - transform.position).normalized;
    }
}
