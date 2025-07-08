using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [Header("DATA")]
    [SerializeField] private NotificationDatabaseSO notificationDatabase;

    [Header("UI")]
    [SerializeField] private Transform spawnArea;
    [SerializeField] private NotificationItemUI itemPrefab;

    public void Populate()
    {
        if (notificationDatabase == null || notificationDatabase.AllNotifications.Count == 0)
        {
            Debug.LogWarning("No notifications found!");
            return;
        }

        // Clear previous entries
        foreach (Transform child in spawnArea)
        {
            Destroy(child.gameObject);
        }

        // Spawn all notifications
        foreach (var entry in notificationDatabase.AllNotifications)
        {
            NotificationItemUI item = Instantiate(itemPrefab, spawnArea);
            item.Configure(entry);
        }
    }
}
