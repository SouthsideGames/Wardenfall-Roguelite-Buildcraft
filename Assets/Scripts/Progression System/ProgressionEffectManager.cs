using UnityEngine;

public class ProgressionEffectManager : MonoBehaviour
{
    public static ProgressionEffectManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ===== CURRENCY EFFECTS =====
    public float MeatMultiplier => HasUnlock("GreedyTouch") ? 1.10f : 1f;
    public int StartingMeat => HasUnlock("EarlyInvestor") ? 200 : 0;
    public int FusionCashReward => HasUnlock("FusionDividend") ? 10 : 0;

    // ===== SHOP EFFECTS =====
    public float ShopDiscount => HasUnlock("MarketSense") ? 0.95f : 1f;
    public bool HasFreeOrDiscountReroll => HasUnlock("DiscountReroll");
    public bool HasExtraShelf => HasUnlock("ExtraShelf");

    // ===== LOOT EFFECTS =====
    public float ChestDropBonus => HasUnlock("ChestMagnet") ? 0.05f : 0f;

    // ===== Centralized Unlock Check =====
    public bool HasUnlock(string unlockID)
    {
        return ProgressionManager.Instance != null &&
               ProgressionManager.Instance.IsUnlockActive(unlockID);
    }
}
