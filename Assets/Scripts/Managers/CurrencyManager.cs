using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("ACTIONS:")]
    public static Action onUpdated;

    [Header("ELEMENTS:")]
    [field: SerializeField] public int Currency { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

    }

    private void Start()
    {
        UpdateUI();
    }

    [Button]
    private void Add500Currency()
    {
        AdjustCurrency(500);
    }

    public void AdjustCurrency(int _amount)
    {
        Currency += _amount;
        UpdateUI();

        onUpdated?.Invoke();
    }

    private void UpdateUI()
    {

        CurrencyUI[] currencyUIs = FindObjectsByType<CurrencyUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (CurrencyUI currencyUI in currencyUIs)
            currencyUI.UpdateText(Currency.ToString());
    }

    public bool HasEnoughCurrency(int _amount) => Currency >= _amount;

    public void UseCurrency(int _amount) => AdjustCurrency(-_amount);
}
