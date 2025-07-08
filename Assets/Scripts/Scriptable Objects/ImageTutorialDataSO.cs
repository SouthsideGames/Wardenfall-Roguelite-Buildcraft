using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial Data", menuName = "Scriptable Objects/Tutorials/New Image Tutorial Data", order = 2)]
public class ImageTutorialDataSO : MonoBehaviour
{
    public TutorialSlideData[] slides;
}

[System.Serializable]
public class TutorialSlideData
{
    public Sprite slideImage;
    [TextArea]
    public string dialogueLine;
}
