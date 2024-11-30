using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectorUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private EventSystem eventSystem;

    private void Awake()
    {
        UIManager.OnPanelShown += PanelSelectCallback;
    }

    private void OnDestroy()
    {
        UIManager.OnPanelShown -= PanelSelectCallback;
    }

    private void PanelSelectCallback(Panel _panel)
    {
        if (_panel.FirstSelectedObject != null)
            SetSelectedGameObject(_panel.FirstSelectedObject);
    }

    private void SetSelectedGameObject(GameObject _go) => eventSystem.SetSelectedGameObject(_go);
}
