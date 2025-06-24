using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWeapon : MonoBehaviour
{

    [Header("ELEMENTS:")]
    [SerializeField] private WeaponPosition[] weaponPositions;

    private Dictionary<string, int> weaponUsageCounts = new Dictionary<string, int>();

    public bool AddWeapon(WeaponDataSO _selectedWeapon, int _weaponLevel)
    {
        int weaponSlotLimit = ChallengeManager.IsActive(ChallengeMode.SingleSlot) ? 1 : weaponPositions.Length;

        for (int i = 0; i < weaponSlotLimit; i++)
        {
            if (weaponPositions[i].Weapon != null)
                continue;

            weaponPositions[i].AssignWeapon(_selectedWeapon.Prefab, _weaponLevel);
            TrackWeaponUsage(_selectedWeapon.ID);
            return true;
        }

        // Optional: log if full during One Weapon Mode
        if (ChallengeManager.IsActive(ChallengeMode.SingleSlot))
        {
            Debug.Log("[OneWeapon] Weapon slot is already filled. Cannot add another.");
        }

        return false;
    }


    private void TrackWeaponUsage(string weaponId)
    {
        if (!weaponUsageCounts.ContainsKey(weaponId))
            weaponUsageCounts[weaponId] = 0;
        weaponUsageCounts[weaponId]++;
    }

    public string GetMostEffectiveWeapon()
    {
        string mostUsedWeaponId = "";
        int maxUsage = 0;

        foreach (var kvp in weaponUsageCounts)
        {
            if (kvp.Value > maxUsage)
            {
                maxUsage = kvp.Value;
                mostUsedWeaponId = kvp.Key;
            }
        }

        return mostUsedWeaponId;
    }

    public Weapon[] GetWeapons()
    {
        List<Weapon> weaponList = new List<Weapon>();  

        foreach (WeaponPosition weaponPosition in weaponPositions)
        {
            if(weaponPosition.Weapon == null)   
                weaponList.Add(null);
            else
                weaponList.Add(weaponPosition.Weapon);
        }

        return weaponList.ToArray();    
    }

    public void RecycleWeapon(int _weaponIndex)
    {
        for(int i = 0; i < weaponPositions.Length; i++)
        {
            if(i != _weaponIndex)
               continue;

            int recyclePrice = weaponPositions[i].Weapon.GetWeaponRecyclePrice();
            CurrencyManager.Instance.AdjustCurrency(recyclePrice);

            weaponPositions[i].RemoveWeaponFromPosition();

            return;
        }
    }
    

    protected virtual void HitStop(bool isCritical = false)
    {
        float stopTime = isCritical ? 0.15f : 0.1f;
        float timeScale = isCritical ? 0.05f : 0.1f;
        Time.timeScale = timeScale;
        StartCoroutine(HitStopCoroutine(stopTime));
    }

    private IEnumerator HitStopCoroutine(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}