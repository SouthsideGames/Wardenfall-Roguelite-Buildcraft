using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangWeapon : RangedWeapon
{
    [Header("BOOMERANG SPECIFICS:")]
    [SerializeField] private float throwDistance;
    [SerializeField] private Bullet boomerangPrefab;

    protected override void Shoot()
    {
        Bullet boomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
        int damage = GetDamage(out bool isCriticalHit);
        
        // Initial throw
        boomerang.Shoot(damage, transform.up, isCriticalHit);
        StartCoroutine(BoomerangReturn(boomerang));
    }

    private IEnumerator BoomerangReturn(Bullet boomerang)
    {
        yield return new WaitForSeconds(0.5f);

        int damage = GetDamage(out bool isCriticalHit);
        Vector2 returnDirection = -boomerang.transform.right;
        
        boomerang.Shoot(damage, returnDirection, isCriticalHit); 
    }
}
