using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatContainerManager : MonoBehaviour
{
    public static StatContainerManager instance;

    [Header("ELEMENTS:")]
    [SerializeField] private StatContainerUI statContainerUI;

    private List<StatContainerUI> statContainerUIs = new List<StatContainerUI>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void GenerateContainers(Dictionary<Stat, float> _statDictionary, Transform _parent)
    {
        statContainerUIs.Clear();

        foreach (KeyValuePair<Stat, float> kvp in _statDictionary)
        {
            StatContainerUI statContainer = Instantiate(statContainerUI, _parent);
            statContainerUIs.Add(statContainer);

            Sprite icon = ResourceManager.GetStatIcon(kvp.Key);
            string statName = Enums.FormatStatName(kvp.Key);
            float statValue = kvp.Value;

            statContainer.Configure(icon, statName, statValue);
        }



        LeanTween.delayedCall(Time.deltaTime * 2, () => ResizeTexts(statContainerUIs));
    }

    private void ResizeTexts(List<StatContainerUI> _statContainerUIs)
    {
        float minFontSize = 5000;

        for (int i = 0; i < _statContainerUIs.Count; i++)
        {
            StatContainerUI statContainerUI = _statContainerUIs[i];
            float fontSize = statContainerUI.GetFontSize();

            if (fontSize < minFontSize)
                minFontSize = fontSize;
        }

        for (int i = 0; i < _statContainerUIs.Count; i++)
        {
            _statContainerUIs[i].SetFontSize(minFontSize);
        }
    }

    public static void GenerateStatContainers(Dictionary<Stat, float> _statDictionary, Transform _parent)
    {
        _parent.Clear();
        instance.GenerateContainers(_statDictionary, _parent);
    }
    
    public void SetMinFontSize(float minFontSize)
    {
        for (int i = 0; i < statContainerUIs.Count; i++)
        {
            statContainerUIs[i].SetFontSize(minFontSize);
        }
    }
}
