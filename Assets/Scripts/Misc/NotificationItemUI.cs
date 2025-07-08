using UnityEngine;
using TMPro;

public class NotificationItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI bodyText;

    public void Configure(NotificationEntrySO entry)
    {
        titleText.text = $"Update {entry.Version}";
        dateText.text = entry.Date;
        bodyText.text = entry.Content;
    }
}
