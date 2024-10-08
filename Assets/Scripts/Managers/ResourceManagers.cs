using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceManagers 
{
    const string statIconsDataPath = "Data/Stat Icons";

    private static StatIcon[] statIcons;

    public static Sprite GetStatIcon(Stat _stat)
    {
        if(statIcons == null)
        {
            StatIconDataSO data = Resources.Load<StatIconDataSO>(statIconsDataPath);
            statIcons = data.StatIcons;
        }
      

        return null;
    }
}
