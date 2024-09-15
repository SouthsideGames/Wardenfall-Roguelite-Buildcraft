using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{

    [Header("Elements")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Bullet bullet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AutoAim();
    }

    protected override void AutoAim()
    {
        base.AutoAim();

        if(closestEnemy != null)
        {
            targetUpVector = (closestEnemy.transform.position - transform.position).normalized;
            transform.up = targetUpVector;  
           
            ShootLogic();
            return;
        }

         transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * aimLerp);

    }

    private void ShootLogic()
    {
        attackTimer += Time.deltaTime;

        if(attackTimer > attackDelay){
            attackTimer = 0f;
            Shoot();
        }
    }

    private void Shoot()
    {
        Bullet _bullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
        _bullet.Shoot(damage, transform.up);
    }

}
