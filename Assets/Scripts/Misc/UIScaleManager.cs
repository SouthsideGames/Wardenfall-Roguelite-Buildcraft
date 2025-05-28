using UnityEngine;
using UnityEngine.UI;

public class UIScaleManager : MonoBehaviour
{
    [Header("Screen Size Thresholds")]
    [SerializeField] private int smallScreenWidth = 800;
    [SerializeField] private int mediumScreenWidth = 1200;
    
    [Header("Scale Factors")]
    [SerializeField] private float mobileScaleFactor = 1.0f;
    [SerializeField] private float tabletScaleFactor = 1.2f;
    [SerializeField] private float desktopScaleFactor = 1.5f;
    
    [Header("Font Size Multipliers")]
    [SerializeField] private float mobileFontMultiplier = 1.0f;
    [SerializeField] private float tabletFontMultiplier = 1.1f;
    [SerializeField] private float desktopFontMultiplier = 1.2f;
    
    [Header("Components to Scale")]
    [SerializeField] private CanvasScaler[] canvasScalers;
    [SerializeField] private Transform[] uiPanels;
    

    
    public static UIScaleManager Instance { get; private set; }
    public DeviceType CurrentDeviceType { get; private set; }
    
    private Vector2 lastScreenSize;
    private float currentScaleFactor = 1.0f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (canvasScalers == null || canvasScalers.Length == 0)
            {
                canvasScalers = FindObjectsByType<CanvasScaler>(FindObjectsSortMode.None);
            }
            
            ApplyScaling();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
        if (currentScreenSize != lastScreenSize)
        {
            ApplyScaling();
            lastScreenSize = currentScreenSize;
        }
    }
    
    public void ApplyScaling()
    {
        DetermineDeviceType();
        SetScaleFactor();
        ApplyCanvasScaling();
        ApplyFontScaling();
        NotifyUIComponents();
    }
    
    private void DetermineDeviceType()
    {
        int screenWidth = Mathf.Max(Screen.width, Screen.height);
        
        if (screenWidth <= smallScreenWidth)
        {
            CurrentDeviceType = DeviceType.Mobile;
        }
        else if (screenWidth <= mediumScreenWidth)
        {
            CurrentDeviceType = DeviceType.Tablet;
        }
        else
        {
            CurrentDeviceType = DeviceType.Desktop;
        }
        
        // Additional mobile detection
        if (Application.isMobilePlatform)
        {
            CurrentDeviceType = screenWidth <= mediumScreenWidth ? DeviceType.Mobile : DeviceType.Tablet;
        }
    }
    
    private void SetScaleFactor()
    {
        switch (CurrentDeviceType)
        {
            case DeviceType.Mobile:
                currentScaleFactor = mobileScaleFactor;
                break;
            case DeviceType.Tablet:
                currentScaleFactor = tabletScaleFactor;
                break;
            case DeviceType.Desktop:
                currentScaleFactor = desktopScaleFactor;
                break;
        }
    }
    
    private void ApplyCanvasScaling()
    {
        foreach (var scaler in canvasScalers)
        {
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = GetReferenceResolution();
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = GetMatchValue();
                scaler.scaleFactor = currentScaleFactor;
            }
        }
    }
    
    private Vector2 GetReferenceResolution()
    {
        switch (CurrentDeviceType)
        {
            case DeviceType.Mobile:
                return new Vector2(1080, 1920);
            case DeviceType.Tablet:
                return new Vector2(1200, 1600);
            case DeviceType.Desktop:
                return new Vector2(1920, 1080);
            default:
                return new Vector2(1080, 1920);
        }
    }
    
    private float GetMatchValue() => 1.0f; 
    
    private void ApplyFontScaling()
    {
        float fontMultiplier = GetFontMultiplier();
        
        if (StatContainerManager.instance != null)
        {
            float baseFontSize = 24f; 
            float scaledFontSize = baseFontSize * fontMultiplier;
            StatContainerManager.instance.SetMinFontSize(scaledFontSize);
        }
    }
    
    private float GetFontMultiplier()
    {
        switch (CurrentDeviceType)
        {
            case DeviceType.Mobile:
                return mobileFontMultiplier;
            case DeviceType.Tablet:
                return tabletFontMultiplier;
            case DeviceType.Desktop:
                return desktopFontMultiplier;
            default:
                return 1.0f;
        }
    }
    
    private void NotifyUIComponents()
    {
        var monoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var mb in monoBehaviours)
        {
            if (mb is IUIAdaptive adaptive)
            {
                adaptive.OnScreenSizeChanged(CurrentDeviceType, currentScaleFactor);
            }
        }
    }
    
    public float GetCurrentScaleFactor() => currentScaleFactor;
    public DeviceType GetDeviceType() => CurrentDeviceType;
}

