using TMPro;
using UnityEngine;

public class LoreDetailUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private GameObject panel;

    public void Open(LoreShardSO shard)
    {
        titleText.text = shard.title;
        bodyText.text = shard.body;
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}
