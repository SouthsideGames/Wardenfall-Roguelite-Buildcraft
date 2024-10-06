using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWeapon : MonoBehaviour
{
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
        Debug.Log("We selected weapon " + _selectedWeapon.Name + " with level : " + _weaponLevel );
    }
}
