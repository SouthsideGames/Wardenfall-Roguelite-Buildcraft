using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ViewerRatingUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider viewerSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI ratingText;

    private void Awake()
    {
        UpdateBar(0.5f);
    }

    public void UpdateBar(float value)
    {
        value = Mathf.Clamp01(value);

        viewerSlider.value = value;
        ratingText.text = $"{Mathf.RoundToInt(value * 100f)}%";

        if (value < 0.4f)
            fillImage.color = Color.red;
        else if (value < 0.7f)
            fillImage.color = Color.yellow;
        else
            fillImage.color = Color.green;
    }
}