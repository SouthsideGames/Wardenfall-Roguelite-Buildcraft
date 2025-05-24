using System.Collections.Generic;
using UnityEngine;

public class ProgressionEffectManager : MonoBehaviour
{
    private Dictionary<string, bool> _unlockCache = new Dictionary<string, bool>();

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
    
    public void ClearCache() => _unlockCache.Clear();
    
    public bool HasUnlock(string unlockID)
    {
        if (string.IsNullOrEmpty(unlockID)) return false;
        
        if (_unlockCache.TryGetValue(unlockID, out bool cached))
            return cached;
            
        bool hasUnlock = ProgressionManager.Instance != null && 
                        ProgressionManager.Instance.IsUnlockActive(unlockID);
                        
        _unlockCache[unlockID] = hasUnlock;
        return hasUnlock;
    }

    private void OnEnable()
    {
        if (ProgressionManager.Instance != null)
        {
            ProgressionManager.OnUnlockPointsChanged += ClearCache;
        }
    }

    private void OnDisable()
    {
        if (ProgressionManager.Instance != null)
        {
            ProgressionManager.OnUnlockPointsChanged -= ClearCache;
        }
    }
}
