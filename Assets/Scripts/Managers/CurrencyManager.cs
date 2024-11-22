using System;
using NaughtyAttributes;
using UnityEngine;
using SouthsideGames.SaveManager;

public class CurrencyManager : MonoBehaviour, IWantToBeSaved
{
    public static CurrencyManager Instance;

    [Header("ACTIONS:")]
    public static Action onCurrencyUpdate;

    private const string premiumCurrencyKey = "PremiumCurrency";

    [Header("ELEMENTS:")]
    [field: SerializeField] public int Currency { get; private set; }
    [field: SerializeField] public int PremiumCurrency { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Candy.OnCollected += CandyCollectedCallback;
        Cash.onCollected += CashCollectedCallback;  

    }

    private void OnDestroy() 
    {
        Candy.OnCollected -= CandyCollectedCallback;
        Cash.onCollected -= CashCollectedCallback;  
    }

    private void Start()=> UpdateUI();
    [Button]
    private void Add500Currency() => AdjustCurrency(500);
    [Button]
    private void Add500PremiumCurrency() => AdjustPremiumCurrency(500);

    public void AdjustCurrency(int _amount)
    {
        Currency += _amount;
        UpdateVisuals();
    }

    public void AdjustPremiumCurrency(int _amount, bool save = true)
    {
        PremiumCurrency += _amount;
        UpdateVisuals();
    }  

    private void UpdateUI()
    {

        CurrencyUI[] currencyUIs = FindObjectsByType<CurrencyUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (CurrencyUI currencyUI in currencyUIs)
            currencyUI.UpdateText(Currency.ToString());

        PremiumCurrencyUI[] premiumCurrencyUIs = FindObjectsByType<PremiumCurrencyUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (PremiumCurrencyUI premiumCurrencyUI in premiumCurrencyUIs)
            premiumCurrencyUI.UpdateText(PremiumCurrency.ToString());
    }

    
    private void UpdateVisuals()
    {
        UpdateUI();
        onCurrencyUpdate?.Invoke();
        Save();
    }

    public void Load()
    {
        if(SaveManager.TryLoad(this, premiumCurrencyKey, out object premiumCurrencyValue))
            AdjustPremiumCurrency((int)premiumCurrencyValue, false);
        else
            AdjustPremiumCurrency(100, false);
    }

    public void Save() => SaveManager.Save(this, premiumCurrencyKey, PremiumCurrency);
    public bool HasEnoughCurrency(int _amount) => Currency >= _amount;
    public void UseCurrency(int _amount) => AdjustCurrency(-_amount);
    public bool HasEnoughPremiumCurrency(int _amount) => PremiumCurrency >= _amount;
    public void UsePremiumCurrency(int _amount) => AdjustPremiumCurrency(-_amount);
    private void CandyCollectedCallback(Candy _candy) => AdjustCurrency(1);
    private void CashCollectedCallback(Cash _case) => AdjustPremiumCurrency(1);
}
