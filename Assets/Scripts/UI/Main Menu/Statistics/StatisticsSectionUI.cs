using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatisticsSectionUI : MonoBehaviour
{
    [SerializeField] private Transform collectionParent;
    [SerializeField] private StatisticsContainerUI weaponCollectionContainerUI;
    [SerializeField] private StatisticsContainerUI characterCollectionContainerUI;

    private void Start()
    {
        PopulateCharacterCollectionSection();
    }

    public void PopulateCharacterCollectionSection()
    {
        collectionParent.Clear();

        foreach (var characterUsage in StatisticsManager.Instance.currentStatistics.CharacterUsageList)
        {
            CharacterDataSO characterData = FindCharacterByID(characterUsage.CharacterID);
            if (characterData != null)
            {
                StatisticsContainerUI instance = Instantiate(characterCollectionContainerUI, collectionParent);
                instance.Configure(characterData.Icon, characterData.Name, characterUsage.UsageInfo.UsageCount, characterUsage.WavesCompleted, characterUsage.UsageInfo.LastUsed);
            }
        }
    }

    public void PopulateWeaponCollectionSection()
    {
        collectionParent.Clear();

        foreach (var weaponUsage in StatisticsManager.Instance.currentStatistics.WeaponUsageList)
        {
            WeaponDataSO weaponData = FindWeaponByID(weaponUsage.WeaponID);
            if (weaponData != null)
            {
                StatisticsContainerUI instance = Instantiate(weaponCollectionContainerUI, collectionParent);
                instance.Configure(weaponData.Icon, weaponData.Name, weaponUsage.UsageInfo.UsageCount, weaponUsage.WavesCompleted, weaponUsage.UsageInfo.LastUsed);
            }
        }
    }

    private CharacterDataSO FindCharacterByID(string id)
    {
        return ResourceManager.Characters.FirstOrDefault(c => c.ID == id);
    }

    private WeaponDataSO FindWeaponByID(string id)
    {
        return ResourceManager.Weapons.FirstOrDefault(w => w.ID == id);
    }
}