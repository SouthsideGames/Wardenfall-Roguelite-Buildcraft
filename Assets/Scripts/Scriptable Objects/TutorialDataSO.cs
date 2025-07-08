
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial Data", menuName = "Scriptable Objects/Tutorials/New Tutorial Data", order = 1)]
public class TutorialDataSO : ScriptableObject
{
    [TextArea(3, 10)]
    public string[] dialogueLines;
}
