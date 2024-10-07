using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWeapon : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private WeaponPosition[] weaponPositions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TryAddWeapon(WeaponDataSO _selectedWeapon, int _weaponLevel)
    {
        weaponPositions[Random.Range(0, weaponPositions.Length)].AssignWeapon(_selectedWeapon.Prefab, _weaponLevel);
    }
}
