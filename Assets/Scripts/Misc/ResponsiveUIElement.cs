using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResponsiveUIElement : MonoBehaviour, IUIAdaptive
{
    [Header("Size Settings")]
    [SerializeField] private Vector2 mobileSize = Vector2.one;
    [SerializeField] private Vector2 tabletSize = Vector2.one;
    [SerializeField] private Vector2 desktopSize = Vector2.one;
    
    [Header("Position Settings")]
    [SerializeField] private Vector2 mobilePosition = Vector2.zero;
    [SerializeField] private Vector2 tabletPosition = Vector2.zero;
    [SerializeField] private Vector2 desktopPosition = Vector2.zero;
    
    [Header("Font Settings")]
    [SerializeField] private float mobileFontSize = 16f;
    [SerializeField] private float tabletFontSize = 20f;
    [SerializeField] private float desktopFontSize = 24f;
    
    [Header("Spacing Settings")]
    [SerializeField] private float mobileSpacing = 10f;
    [SerializeField] private float tabletSpacing = 15f;
    [SerializeField] private float desktopSpacing = 20f;
    
    private RectTransform rectTransform;
    private TextMeshProUGUI textComponent;
    private Text legacyTextComponent;
    private LayoutGroup layoutGroup;
    private ContentSizeFitter contentSizeFitter;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        textComponent = GetComponent<TextMeshProUGUI>();
        legacyTextComponent = GetComponent<Text>();
        layoutGroup = GetComponent<LayoutGroup>();
        contentSizeFitter = GetComponent<ContentSizeFitter>();
        
        // Apply initial scaling
        if (UIScaleManager.Instance != null)
        {
            OnScreenSizeChanged(UIScaleManager.Instance.GetDeviceType(), 
                               UIScaleManager.Instance.GetCurrentScaleFactor());
        }
    }
    
    public void OnScreenSizeChanged(UIScaleManager.DeviceType deviceType, float scaleFactor)
    {
        ApplySize(deviceType);
        ApplyPosition(deviceType);
        ApplyFontSize(deviceType);
        ApplySpacing(deviceType);
    }
    
    private void ApplySize(UIScaleManager.DeviceType deviceType)
    {
        if (rectTransform == null) return;
        
        Vector2 targetSize = deviceType switch
        {
            UIScaleManager.DeviceType.Mobile => mobileSize,
            UIScaleManager.DeviceType.Tablet => tabletSize,
            UIScaleManager.DeviceType.Desktop => desktopSize,
            _ => mobileSize
        };
        
        if (targetSize != Vector2.one) // Only apply if not default
        {
            rectTransform.sizeDelta = targetSize;
        }
    }
    
    private void ApplyPosition(UIScaleManager.DeviceType deviceType)
    {
        if (rectTransform == null) return;
        
        Vector2 targetPosition = deviceType switch
        {
            UIScaleManager.DeviceType.Mobile => mobilePosition,
            UIScaleManager.DeviceType.Tablet => tabletPosition,
            UIScaleManager.DeviceType.Desktop => desktopPosition,
            _ => mobilePosition
        };
        
        if (targetPosition != Vector2.zero) // Only apply if not default
        {
            rectTransform.anchoredPosition = targetPosition;
        }
    }
    
    private void ApplyFontSize(UIScaleManager.DeviceType deviceType)
    {
        float targetSize = deviceType switch
        {
            UIScaleManager.DeviceType.Mobile => mobileFontSize,
            UIScaleManager.DeviceType.Tablet => tabletFontSize,
            UIScaleManager.DeviceType.Desktop => desktopFontSize,
            _ => mobileFontSize
        };
        
        if (textComponent != null && targetSize > 0)
        {
            textComponent.fontSize = targetSize;
        }
        
        if (legacyTextComponent != null && targetSize > 0)
        {
            legacyTextComponent.fontSize = Mathf.RoundToInt(targetSize);
        }
    }
    
    private void ApplySpacing(UIScaleManager.DeviceType deviceType)
    {
        float targetSpacing = deviceType switch
        {
            UIScaleManager.DeviceType.Mobile => mobileSpacing,
            UIScaleManager.DeviceType.Tablet => tabletSpacing,
            UIScaleManager.DeviceType.Desktop => desktopSpacing,
            _ => mobileSpacing
        };
        
        if (layoutGroup != null && targetSpacing > 0)
        {
            if (layoutGroup is HorizontalLayoutGroup hlg)
            {
                hlg.spacing = targetSpacing;
            }
            else if (layoutGroup is VerticalLayoutGroup vlg)
            {
                vlg.spacing = targetSpacing;
            }
            else if (layoutGroup is GridLayoutGroup glg)
            {
                glg.spacing = new Vector2(targetSpacing, targetSpacing);
            }
        }
    }
}
