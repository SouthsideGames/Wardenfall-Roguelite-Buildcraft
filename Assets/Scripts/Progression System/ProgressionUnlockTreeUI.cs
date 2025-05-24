using UnityEngine;
using TMPro;

public class ProgressionUnlockTreeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText;

    public void Refresh()
    {
        pointsText.text = $"Points Available: {ProgressionManager.Instance.UnlockPoints}";
    
    }
}
