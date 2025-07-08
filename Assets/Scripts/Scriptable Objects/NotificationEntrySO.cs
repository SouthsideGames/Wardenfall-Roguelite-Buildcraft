using UnityEngine;

[CreateAssetMenu(fileName = "Notification Data", menuName = "Scriptable Objects/Notifications/New Notification Entry", order = 2)]
public class NotificationEntrySO : ScriptableObject
{
    public string Version;
    public string Date;
    [TextArea(5, 30)]
    public string Content;
}
