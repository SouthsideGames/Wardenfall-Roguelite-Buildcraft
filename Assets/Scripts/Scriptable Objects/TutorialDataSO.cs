
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial", menuName = "Tutorial/Tutorial Data")]
public class TutorialDataSO : ScriptableObject
{
    [TextArea(3, 10)]
    public string[] dialogueLines;
    public string panelId;
}
