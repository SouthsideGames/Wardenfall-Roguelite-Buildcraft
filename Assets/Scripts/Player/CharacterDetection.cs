using UnityEngine;
using System.Collections;   
using System.Collections.Generic;   

public class CharacterDetection : MonoBehaviour
{
    [Header("COLLIDERS:")]
    [SerializeField] private Collider2D collectableCollider;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.TryGetComponent(out ICollectable _collectable))
        {
            if(!collider.IsTouching(collectableCollider))
                return;

            _collectable.Collect(GetComponent<CharacterManager>());
        }
    }
}
