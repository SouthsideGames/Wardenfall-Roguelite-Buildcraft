
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ColorblindUIElement : MonoBehaviour
{
    [Header("Colorblind Accessibility")]
    [SerializeField] private string colorType = "primary";
    [SerializeField] private bool autoRegister = true;
    
    private void Start()
    {
        if (autoRegister && ColorblindAccessibilityManager.Instance != null)
        {
            RegisterWithManager();
        }
    }
    
    private void RegisterWithManager()
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            ColorblindAccessibilityManager.Instance.RegisterImage(image, colorType);
        }
        
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            ColorblindAccessibilityManager.Instance.RegisterText(text, colorType);
        }
    }
    
    [System.Serializable]
    public enum ColorType
    {
        Primary,
        Secondary,
        Positive,
        Negative,
        Warning,
        Neutral,
        Health,
        Mana,
        Damage,
        Critical,
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}
