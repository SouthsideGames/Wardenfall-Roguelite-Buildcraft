using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsDisplayUI : MonoBehaviour, IStats
{
    [Header("ELEMENTS:")]
    [SerializeField] private Transform characterStatContainersParent;

    public void UpdateStats(CharacterStats _statsManager)
    {
        int index = 0;

        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            StatContainerUI statContainerUI = characterStatContainersParent.GetChild(index).GetComponent<StatContainerUI>();
            statContainerUI.gameObject.SetActive(true);

            Sprite statSprite = ResourceManager.GetStatIcon(stat);
            string statValue = _statsManager.GetStatValue(stat).ToString("F2");

            statContainerUI.Configure(statSprite, Enums.FormatStatName(stat), statValue);

            index++;
        }
        
        for(int i = index; i < characterStatContainersParent.childCount; i++)
            characterStatContainersParent.GetChild(i).gameObject.SetActive(false);
    }
}
