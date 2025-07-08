using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Notification Data", menuName = "Scriptable Objects/Notifications/New Notification Database", order = 1)]
public class NotificationDatabaseSO : ScriptableObject
{
    public List<NotificationEntrySO> AllNotifications;
}
