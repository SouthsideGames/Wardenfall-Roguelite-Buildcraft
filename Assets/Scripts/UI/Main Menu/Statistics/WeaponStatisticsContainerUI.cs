using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponStatisticsContainerUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI timesUsedText;
    [SerializeField] private TextMeshProUGUI highestDamageText;

    public void Configure(Sprite _icon, string _name, int _timesUsed, float _highestDamage)
    {
        weaponIcon.sprite = _icon;
        weaponNameText.text = _name;
        timesUsedText.text = $"Times Used: {_timesUsed}";
        highestDamageText.text = $"Highest Damage: {_highestDamage:F0}";
    }
}
