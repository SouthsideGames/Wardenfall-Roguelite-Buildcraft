
using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea = Rect.zero;
    private Vector2 anchorMin;
    private Vector2 anchorMax;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        anchorMin = rectTransform.anchorMin;
        anchorMax = rectTransform.anchorMax;
        ApplySafeArea();
    }

    void Update()
    {
        if (lastSafeArea != Screen.safeArea)
        {
            ApplySafeArea();
        }
    }

    void ApplySafeArea()
    {
        if (rectTransform == null)
            return;

        Rect safeArea = Screen.safeArea;
        Vector2 anchorMinNew = safeArea.position;
        Vector2 anchorMaxNew = safeArea.position + safeArea.size;
        anchorMinNew.x /= Screen.width;
        anchorMinNew.y /= Screen.height;
        anchorMaxNew.x /= Screen.width;
        anchorMaxNew.y /= Screen.height;

        rectTransform.anchorMin = new Vector2(anchorMin.x + anchorMinNew.x, anchorMin.y + anchorMinNew.y);
        rectTransform.anchorMax = new Vector2(anchorMax.x * anchorMaxNew.x, anchorMax.y * anchorMaxNew.y);

        lastSafeArea = Screen.safeArea;
    }
}
