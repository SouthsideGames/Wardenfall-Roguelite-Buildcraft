using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceManager 
{
    const string statIconsDataPath = "Data/Stat Icons";
    const string objectDatasPath = "Data/Objects/";

    private static StatIcon[] statIcons;

    public static Sprite GetStatIcon(Stat _stat)
    {
        if(statIcons == null)
        {
            StatIconDataSO data = Resources.Load<StatIconDataSO>(statIconsDataPath);
            statIcons = data.StatIcons;
        }
      
        foreach(StatIcon icon in statIcons)
            if(_stat == icon.stat)
              return icon.icon;

        Debug.LogError("No icon found for stat : " + _stat);

        return null;
    }

    private static ObjectDataSO[] objectDatas;
    public static ObjectDataSO[] Objects
    {
        get 
        { 
            if(objectDatas == null)
                objectDatas = Resources.LoadAll<ObjectDataSO>(objectDatasPath);

            return objectDatas;

        }
        private set {}
    }
}
