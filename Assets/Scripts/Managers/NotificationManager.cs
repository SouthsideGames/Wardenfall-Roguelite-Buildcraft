using UnityEngine;
using SouthsideGames.SaveManager;

public class NotificationManager : MonoBehaviour, IWantToBeSaved
{
    private const string LastReadKey = "LastReadNotificationVersion";

    [Header("DATA")]
    [SerializeField] private NotificationDatabaseSO notificationDatabase;

    [Header("UI")]
    [SerializeField] private Transform spawnArea;
    [SerializeField] private NotificationItemUI itemPrefab;
    [SerializeField] private GameObject alertIcon;

    private string lastReadVersion = "";

    private void OnEnable()
    {
        MarkLatestAsRead();
        Populate();
    }

    private void Start()
    {
        Populate();
        UpdateAlertIcon();
    }

    public void Populate()
    {
        if (notificationDatabase == null || notificationDatabase.AllNotifications.Count == 0)
        {
            Debug.LogWarning("No notifications found!");
            return;
        }

        foreach (Transform child in spawnArea)
        {
            Destroy(child.gameObject);
        }

        foreach (var entry in notificationDatabase.AllNotifications)
        {
            var item = Instantiate(itemPrefab, spawnArea);
            item.Configure(entry);
        }
    }

    private void MarkLatestAsRead()
    {
        if (notificationDatabase == null || notificationDatabase.AllNotifications.Count == 0)
            return;

        var latest = notificationDatabase.AllNotifications[^1];
        if (latest.Version != lastReadVersion)
        {
            lastReadVersion = latest.Version;
            Save();
        }

        UpdateAlertIcon();
    }

    private void UpdateAlertIcon()
    {
        if (alertIcon == null || notificationDatabase == null || notificationDatabase.AllNotifications.Count == 0)
        {
            if (alertIcon != null) alertIcon.SetActive(false);
            return;
        }

        var latestVersion = notificationDatabase.AllNotifications[^1].Version;
        bool hasUnread = latestVersion != lastReadVersion;

        alertIcon.SetActive(hasUnread);
    }

    #region IWantToBeSaved Implementation
    public void Load()
    {
        if (SaveManager.TryLoad(this, LastReadKey, out object value))
        {
            lastReadVersion = value as string ?? "";
        }
        else
        {
            lastReadVersion = "";
        }

        UpdateAlertIcon();
    }

    public void Save()
    {
        SaveManager.Save(this, LastReadKey, lastReadVersion);
    }
    #endregion
}
