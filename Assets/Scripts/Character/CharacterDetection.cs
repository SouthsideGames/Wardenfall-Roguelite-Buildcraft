using UnityEngine;
using System.Collections;   
using System.Collections.Generic;   

public class CharacterDetection : MonoBehaviour, IStats
{
    [Header("COLLIDERS:")]
    [SerializeField] private CircleCollider2D collectableCollider;
    [SerializeField] private float basePickupRange;
    private float pickupRange;

    private void Start() => IncreaseColliderRadius();   

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.TryGetComponent(out ICollectable _collectable))
        {
            if(!collider.IsTouching(collectableCollider))
                return;

            _collectable.Collect(GetComponent<CharacterManager>());
        }
    }

    public void UpdateWeaponStats(CharacterStats _statsManager)
    {
        float pickUpRangePercent = _statsManager.GetStatValue(Stat.PickupRange) / 100;
        pickupRange = basePickupRange * (1 + pickUpRangePercent);

        IncreaseColliderRadius();
    }

    private void IncreaseColliderRadius()
    {
        collectableCollider.radius = pickupRange;
    }
}
