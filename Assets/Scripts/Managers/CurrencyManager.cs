using System;
using NaughtyAttributes;
using UnityEngine;
using SouthsideGames.SaveManager;
using UnityEngine.Purchasing;

public class CurrencyManager : MonoBehaviour, IWantToBeSaved
{
    public static CurrencyManager Instance;

    [Header("ACTIONS:")]
    public static Action onCurrencyUpdate;

    private const string premiumCurrencyKey = "PremiumCurrency";
    private const string cardCurrencyKey = "CardCurrency";

    [Header("ELEMENTS:")]
    [field: SerializeField] public int Currency { get; private set; }
    [field: SerializeField] public int PremiumCurrency { get; private set; }
    [field: SerializeField] public int CardCurrency { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Meat.OnCollected += MeatCollectedCallback;
        Cash.OnCollected += CashCollectedCallback;
        CardPoint.OnCollected += CardPointsCollectedCallback;

    }

    private void OnDestroy()
    {
        Meat.OnCollected -= MeatCollectedCallback;
        Cash.OnCollected -= CashCollectedCallback;
        CardPoint.OnCollected -= CardPointsCollectedCallback;
    }

    private void Start() => UpdateUI();

    [Button]
    private void Add500Currency() => AdjustCurrency(500);
    [Button]
    private void Add500PremiumCurrency() => AdjustPremiumCurrency(500);
    [Button]
    private void Add500CardPoints() => AdjustCardCurrency(500);

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


    public void AdjustCardCurrency(int _amount, bool save = true)
    {
        CardCurrency += _amount;
        UpdateVisuals();
    }


    public void AdjustCardPoints(int _amount)
    {
        CardCurrency += _amount;
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

        CardCurrencyUI[] cardCurrencyUIs = FindObjectsByType<CardCurrencyUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (CardCurrencyUI cardCurrencyUI in cardCurrencyUIs)
            cardCurrencyUI.UpdateText(CardCurrency.ToString());

    }

    private void UpdateVisuals()
    {
        UpdateUI();
        onCurrencyUpdate?.Invoke();
        Save();
    }

    public void Load()
    {
        if (SaveManager.TryLoad(this, premiumCurrencyKey, out object premiumCurrencyValue))
            AdjustPremiumCurrency((int)premiumCurrencyValue, false);
        else
            AdjustPremiumCurrency(100, false);

        if (SaveManager.TryLoad(this, cardCurrencyKey, out object cardCurrencyValue))
            AdjustCardCurrency((int)cardCurrencyValue, false);
        else
            AdjustCardCurrency(100, false);
    }

    public void EarlyInvestorSkillAction()
    {
        int startingMeat = ProgressionEffectManager.Instance.StartingMeat;
        if (startingMeat > 0)
            AdjustCurrency(startingMeat);
    }

    public void Save()
    {
        SaveManager.Save(this, premiumCurrencyKey, PremiumCurrency);
        SaveManager.Save(this, cardCurrencyKey, CardCurrency);
    }

    public bool HasEnoughCurrency(int _amount) => Currency >= _amount;
    public void UseCurrency(int _amount) => AdjustCurrency(-_amount);

    public bool HasEnoughPremiumCurrency(int _amount) => PremiumCurrency >= _amount;
    public void UsePremiumCurrency(int _amount) => AdjustPremiumCurrency(-_amount);
    public bool HasEnoughCardCurrency(int _amount) => CardCurrency >= _amount;
    public void UseCardCurrency(int _amount) => AdjustCardCurrency(-_amount);
    public bool HasEnoughCardPoints(int _amount) => CardCurrency >= _amount;
    public void UseCardPoints(int _amount) => AdjustCardPoints(-_amount);

    private void MeatCollectedCallback(Meat _meat)
    {
        float multiplier = ProgressionEffectManager.Instance != null ? ProgressionEffectManager.Instance.MeatMultiplier : 1f;
        int reward = Mathf.RoundToInt(1 * multiplier);
        AdjustCurrency(reward);
    }

    private void CashCollectedCallback(Cash _cash) => AdjustPremiumCurrency(1);
    private void CardPointsCollectedCallback(CardPoint _cardPoint) => AdjustCardCurrency(1);
    
}