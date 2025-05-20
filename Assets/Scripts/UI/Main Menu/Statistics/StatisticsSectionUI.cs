using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatisticsSectionUI : MonoBehaviour
{
    [SerializeField] private Transform collectionParent;
    [SerializeField] private WeaponStatisticsContainerUI weaponStatisticsContainerUI;
    [SerializeField] private CharacterStatisticsContainerUI characterStatisticsContainerUI;

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
                CharacterStatisticsContainerUI instance = Instantiate(characterStatisticsContainerUI, collectionParent);
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
                WeaponStatisticsContainerUI instance = Instantiate(weaponStatisticsContainerUI, collectionParent);
                instance.Configure(weaponData.Icon, weaponData.Name, weaponUsage.TimesUsed, weaponUsage.HighestDamageDealt);
            }
        }
    }


    private CharacterDataSO FindCharacterByID(string id) => ResourceManager.Characters.FirstOrDefault(c => c.ID == id);

    private WeaponDataSO FindWeaponByID(string id) => ResourceManager.Weapons.FirstOrDefault(w => w.ID == id);
}