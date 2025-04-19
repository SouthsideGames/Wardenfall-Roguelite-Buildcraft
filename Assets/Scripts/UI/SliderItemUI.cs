using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SouthsideGames.DailyMissions
{
    [RequireComponent(typeof(Button))]
    public class SliderItemUI : MonoBehaviour
    {
        [Header("ELEMENTS:")]
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;
        public TextMeshProUGUI Text => text;    

        private Button button;
        public Button Button => button; 

        public void Configure(Sprite _sprite, string _label)
        {
            image.sprite = _sprite; 
            text.text = _label; 

            button = GetComponent<Button>();
        }

        [NaughtyAttributes.Button]
        public void Animate()
        {
            LeanTween.cancel(image.gameObject);
            LeanTween.rotateLocal(image.gameObject, Vector3.forward * 10, .5f).setLoopPingPong(100);
        }

        [NaughtyAttributes.Button]
        public void StopAnimation()
        {
            LeanTween.cancel(image.gameObject);
            LeanTween.rotate(image.gameObject, Vector3.zero, .5f);
        }
    }

}
