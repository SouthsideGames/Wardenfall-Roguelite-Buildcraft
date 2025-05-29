
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial Data", menuName = "Scriptable Objects/New Tutorial Data", order = 10)]
public class TutorialDataSO : ScriptableObject
{
    [TextArea(3, 10)]
    public string[] dialogueLines;
}
