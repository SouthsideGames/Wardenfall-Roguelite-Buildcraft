using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    [Header("Safe Area Settings")]
    [SerializeField] private bool applyToX = true;
    [SerializeField] private bool applyToY = true;
    [SerializeField] private bool conformX = true;
    [SerializeField] private bool conformY = true;
    
    private RectTransform rectTransform;
    private Rect lastSafeArea = Rect.zero;
    private Vector2 lastScreenSize = Vector2.zero;
    private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        lastOrientation = Screen.orientation;
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        ApplySafeArea();
    }

    void Update()
    {
        if (HasChanged())
        {
            ApplySafeArea();
        }
    }

    private bool HasChanged()
    {
        return lastSafeArea != Screen.safeArea || 
               lastScreenSize.x != Screen.width || 
               lastScreenSize.y != Screen.height ||
               lastOrientation != Screen.orientation;
    }

    void ApplySafeArea()
    {
        if (rectTransform == null)
            return;

        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        if (conformX)
        {
            if (applyToX)
            {
                rectTransform.anchorMin = new Vector2(anchorMin.x, rectTransform.anchorMin.y);
                rectTransform.anchorMax = new Vector2(anchorMax.x, rectTransform.anchorMax.y);
            }
        }
        
        if (conformY)
        {
            if (applyToY)
            {
                rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, anchorMin.y);
                rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, anchorMax.y);
            }
        }

        lastSafeArea = Screen.safeArea;
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        lastOrientation = Screen.orientation;
    }
}
