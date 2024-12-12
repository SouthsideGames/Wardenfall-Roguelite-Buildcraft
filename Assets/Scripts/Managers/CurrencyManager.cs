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
    private const string gemCurrencyKey = "GemCurrency";

    [Header("ELEMENTS:")]
    [field: SerializeField] public int Currency { get; private set; }
    [field: SerializeField] public int PremiumCurrency { get; private set; }
    [field: SerializeField] public int GemCurrency { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Meat.OnCollected += MeatCollectedCallback;
        Cash.onCollected += CashCollectedCallback;  
        Gem.OnCollected -= GemCollectedCallback;

    }

    private void OnDestroy() 
    {
        Meat.OnCollected -= MeatCollectedCallback;
        Cash.onCollected -= CashCollectedCallback;  
        Gem.OnCollected -= GemCollectedCallback;
    }

    private void Start()=> UpdateUI();
    [Button]
    private void Add500Currency() => AdjustCurrency(500);
    [Button]
    private void Add500PremiumCurrency() => AdjustPremiumCurrency(500);
    [Button]
    private void Add500GemCurrency() => AdjustGemCurrency(500);

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

    
    public void AdjustGemCurrency(int _amount, bool save = true)
    {
        GemCurrency += _amount;
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

        GemCurrencyUI[] gemCurrencyUIs = FindObjectsByType<GemCurrencyUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (GemCurrencyUI gemCurrencyUI in gemCurrencyUIs)
            gemCurrencyUI.UpdateText(GemCurrency.ToString());
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

        if(SaveManager.TryLoad(this, gemCurrencyKey, out object gemCurrencyValue))
            AdjustGemCurrency((int)gemCurrencyValue, false);
        else
            AdjustGemCurrency(100, false);
    }

    public void Save()
    {
        SaveManager.Save(this, premiumCurrencyKey, PremiumCurrency);
        SaveManager.Save(this, gemCurrencyKey, GemCurrency);
    }

    public bool HasEnoughCurrency(int _amount) => Currency >= _amount;
    public void UseCurrency(int _amount) => AdjustCurrency(-_amount);
    public bool HasEnoughPremiumCurrency(int _amount) => PremiumCurrency >= _amount;
    public void UsePremiumCurrency(int _amount) => AdjustPremiumCurrency(-_amount);
    public void UseGemCurrency(int _amount) => AdjustGemCurrency(-_amount);
    private void MeatCollectedCallback(Meat _meat) => AdjustCurrency(1);
    private void CashCollectedCallback(Cash _cash) => AdjustPremiumCurrency(1);
    private void GemCollectedCallback(Gem _gem) => AdjustGemCurrency(1);    
}
