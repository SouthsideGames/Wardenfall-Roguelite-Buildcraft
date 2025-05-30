using UnityEngine;

public class LorePanelUI : MonoBehaviour
{
    public static LorePanelUI Instance;

    [SerializeField] private Transform shardContainer;
    [SerializeField] private GameObject shardButtonPrefab;
    [SerializeField] private LoreDetailUI detailPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        ClearOldEntries();

        foreach (var shard in LoreManager.Instance.GetUnlockedShards())
        {
            var btn = Instantiate(shardButtonPrefab, shardContainer);
            btn.GetComponent<ShardButtonUI>().Setup(shard, OnShardSelected);
        }
    }

    private void OnShardSelected(LoreShardSO shard)
    {
        detailPanel.Open(shard);
        LoreManager.Instance.MarkAsRead(shard.id);
    }

    private void ClearOldEntries()
    {
        foreach (Transform child in shardContainer)
            Destroy(child.gameObject);
    }
}
