using UnityEngine;

[CreateAssetMenu(fileName = "Image Tutorial Data", menuName = "Scriptable Objects/Tutorials/New Image Tutorial Data", order = 2)]
public class ImageTutorialDataSO : ScriptableObject
{
    public TutorialSlideData[] slides;
}

[System.Serializable]
public class TutorialSlideData
{
    public Sprite slideImage;
    [TextArea]
    public string[] dialogueLines;
}
