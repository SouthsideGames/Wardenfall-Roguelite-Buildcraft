
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ColorblindAccessibilityManager : MonoBehaviour
{
    public static ColorblindAccessibilityManager Instance;

    [Header("COLORBLIND SETTINGS")]
    [SerializeField] private ColorblindPalette normalPalette;
    [SerializeField] private ColorblindPalette colorblindPalette;

    private bool isColorblindModeEnabled = false;
    private Dictionary<Image, string> registeredImages = new Dictionary<Image, string>();
    private Dictionary<TextMeshProUGUI, string> registeredTexts = new Dictionary<TextMeshProUGUI, string>();

    [System.Serializable]
    public class ColorblindPalette
    {
        [Header("UI Colors")]
        public Color primaryColor = Color.white;
        public Color secondaryColor = Color.gray;
        public Color positiveColor = Color.green;
        public Color negativeColor = Color.red;
        public Color warningColor = Color.yellow;
        public Color neutralColor = Color.blue;

        [Header("Health/Status Colors")]
        public Color healthColor = Color.green;
        public Color manaColor = Color.blue;
        public Color damageColor = Color.red;
        public Color criticalColor = Color.yellow;

        [Header("Rarity Colors")]
        public Color commonColor = Color.white;
        public Color uncommonColor = Color.green;
        public Color rareColor = Color.blue;
        public Color epicColor = Color.magenta;
        public Color legendaryColor = Color.yellow;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Set up default palettes if not assigned
        if (normalPalette == null)
            normalPalette = new ColorblindPalette();
        
        if (colorblindPalette == null)
            SetupColorblindPalette();
    }

    private void Start()
    {
        // Subscribe to settings change
        SettingManager.onColorblindSupportChanged += OnColorblindSupportChanged;
        
        // Apply current setting and force initial update
        if (SettingManager.Instance != null)
        {
            isColorblindModeEnabled = SettingManager.Instance.colorblindSupport;
        }
        
        // Auto-register UI elements
        StartCoroutine(AutoRegisterUIElements());
    }
    
    private System.Collections.IEnumerator AutoRegisterUIElements()
    {
        yield return new WaitForEndOfFrame();
        
        // Find and register UI elements with specific tags or components
        RegisterHealthBars();
        RegisterCurrencyElements();
        RegisterStatusElements();
        RegisterButtonElements();
        
        // Apply colorblind mode after registration
        ApplyColorblindMode();
    }
    
    private void RegisterHealthBars()
    {
        // Register health bars and related UI
        Image[] healthImages = FindObjectsByType<Image>(FindObjectsSortMode.None);
        foreach (var img in healthImages)
        {
            if (img.name.ToLower().Contains("health") || img.name.ToLower().Contains("hp"))
            {
                RegisterImage(img, "health");
            }
            else if (img.name.ToLower().Contains("damage") || img.name.ToLower().Contains("hurt"))
            {
                RegisterImage(img, "damage");
            }
        }
    }
    
    private void RegisterCurrencyElements()
    {
        // Register currency-related UI elements
        TextMeshProUGUI[] texts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        foreach (var text in texts)
        {
            if (text.name.ToLower().Contains("currency") || text.name.ToLower().Contains("coin") || text.name.ToLower().Contains("cash"))
            {
                RegisterText(text, "positive");
            }
        }
    }
    
    private void RegisterStatusElements()
    {
        // Register status effect icons and similar elements
        Image[] statusImages = FindObjectsByType<Image>(FindObjectsSortMode.None);
        foreach (var img in statusImages)
        {
            if (img.name.ToLower().Contains("positive") || img.name.ToLower().Contains("buff"))
            {
                RegisterImage(img, "positive");
            }
            else if (img.name.ToLower().Contains("negative") || img.name.ToLower().Contains("debuff"))
            {
                RegisterImage(img, "negative");
            }
            else if (img.name.ToLower().Contains("warning") || img.name.ToLower().Contains("alert"))
            {
                RegisterImage(img, "warning");
            }
        }
    }
    
    private void RegisterButtonElements()
    {
        // Register important buttons with semantic colors
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                if (button.name.ToLower().Contains("confirm") || button.name.ToLower().Contains("yes") || button.name.ToLower().Contains("accept"))
                {
                    RegisterImage(buttonImage, "positive");
                }
                else if (button.name.ToLower().Contains("cancel") || button.name.ToLower().Contains("no") || button.name.ToLower().Contains("decline"))
                {
                    RegisterImage(buttonImage, "negative");
                }
                else if (button.name.ToLower().Contains("warning") || button.name.ToLower().Contains("caution"))
                {
                    RegisterImage(buttonImage, "warning");
                }
            }
        }
    }

    private void OnDestroy()
    {
        SettingManager.onColorblindSupportChanged -= OnColorblindSupportChanged;
    }

    private void SetupColorblindPalette()
    {
        colorblindPalette = new ColorblindPalette
        {
            // High contrast colors for colorblind accessibility
            primaryColor = Color.white,
            secondaryColor = new Color(0.8f, 0.8f, 0.8f),
            positiveColor = new Color(0f, 0.6f, 1f), // Blue instead of green
            negativeColor = new Color(1f, 0.4f, 0f), // Orange instead of red
            warningColor = new Color(1f, 1f, 0f), // Bright yellow
            neutralColor = new Color(0.7f, 0.7f, 0.7f),

            healthColor = new Color(0f, 0.6f, 1f), // Blue
            manaColor = new Color(0.5f, 0f, 1f), // Purple
            damageColor = new Color(1f, 0.4f, 0f), // Orange
            criticalColor = new Color(1f, 1f, 0f), // Yellow

            commonColor = Color.white,
            uncommonColor = new Color(0f, 0.6f, 1f), // Blue
            rareColor = new Color(0.5f, 0f, 1f), // Purple
            epicColor = new Color(1f, 0.4f, 0f), // Orange
            legendaryColor = new Color(1f, 1f, 0f) // Yellow
        };
    }

    private void OnColorblindSupportChanged(bool enabled)
    {
        isColorblindModeEnabled = enabled;
        ApplyColorblindMode();
    }

    public void RegisterImage(Image image, string colorType)
    {
        if (image != null)
        {
            registeredImages[image] = colorType;
            ApplyColorToImage(image, colorType);
        }
    }

    public void RegisterText(TextMeshProUGUI text, string colorType)
    {
        if (text != null)
        {
            registeredTexts[text] = colorType;
            ApplyColorToText(text, colorType);
        }
    }

    private void ApplyColorblindMode()
    {
        foreach (var kvp in registeredImages)
        {
            if (kvp.Key != null)
                ApplyColorToImage(kvp.Key, kvp.Value);
        }

        foreach (var kvp in registeredTexts)
        {
            if (kvp.Key != null)
                ApplyColorToText(kvp.Key, kvp.Value);
        }
        
        Debug.Log($"Applied colorblind mode: {isColorblindModeEnabled}. Registered Images: {registeredImages.Count}, Registered Texts: {registeredTexts.Count}");
    }

    private void ApplyColorToImage(Image image, string colorType)
    {
        Color color = GetColorByType(colorType);
        image.color = color;
    }

    private void ApplyColorToText(TextMeshProUGUI text, string colorType)
    {
        Color color = GetColorByType(colorType);
        text.color = color;
    }

    private Color GetColorByType(string colorType)
    {
        ColorblindPalette palette = isColorblindModeEnabled ? colorblindPalette : normalPalette;

        return colorType.ToLower() switch
        {
            "primary" => palette.primaryColor,
            "secondary" => palette.secondaryColor,
            "positive" => palette.positiveColor,
            "negative" => palette.negativeColor,
            "warning" => palette.warningColor,
            "neutral" => palette.neutralColor,
            "health" => palette.healthColor,
            "mana" => palette.manaColor,
            "damage" => palette.damageColor,
            "critical" => palette.criticalColor,
            "common" => palette.commonColor,
            "uncommon" => palette.uncommonColor,
            "rare" => palette.rareColor,
            "epic" => palette.epicColor,
            "legendary" => palette.legendaryColor,
            _ => Color.white
        };
    }

    

    // Public methods for easy access
    public Color GetHealthColor() => GetColorByType("health");
    public Color GetDamageColor() => GetColorByType("damage");
    public Color GetPositiveColor() => GetColorByType("positive");
    public Color GetNegativeColor() => GetColorByType("negative");
    public Color GetRarityColor(string rarity) => GetColorByType(rarity);
    
    // Get current palette
    public ColorblindPalette GetCurrentPalette() => isColorblindModeEnabled ? colorblindPalette : normalPalette;
    
    // Force update all registered elements
    public void ForceUpdateAllColors()
    {
        ApplyColorblindMode();
    }
}
