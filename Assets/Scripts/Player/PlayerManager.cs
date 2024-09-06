using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterHealth))]
public class PlayerManager : MonoBehaviour
{
    [Header("Components")]
    private CharacterHealth characterHealth;
    

    private void Awake()
    {
        characterHealth = GetComponent<CharacterHealth>();  
    }


    public void TakeDamage(int _damage)
    {
        characterHealth.TakeDamage(_damage);    
    }
}
