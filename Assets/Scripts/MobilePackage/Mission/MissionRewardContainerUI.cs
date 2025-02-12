using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SouthsideGames.DailyMissions
{
    public class MissionRewardContainerUI : MonoBehaviour
    {
        [Header("ELEMENTS:")]
        [SerializeField] private Image rewardImage;
        [SerializeField] private TextMeshProUGUI rewardLabel;

        public void Configure(Sprite _icon, string _label)
        {
            rewardImage.sprite = _icon;
            rewardLabel.text = _label;   
        }
        
    }

}

 