using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TraitEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI stackText;
    [SerializeField] private Image iconImage;

   public void Setup(TraitDataSO trait, int stack)
    {
        nameText.text = trait.GetTier(stack).TierName;
        descriptionText.text = trait.GetTier(stack).Description;
        stackText.text = $"x{stack}";

        if (iconImage != null && trait.Icon != null)
            iconImage.sprite = trait.Icon;
    }


}
