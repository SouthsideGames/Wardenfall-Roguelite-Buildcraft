using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShardButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject newTag;

    private LoreShardSO shardData;
    private Action<LoreShardSO> onClickCallback;

    public void Setup(LoreShardSO shard, Action<LoreShardSO> onClick)
    {
        shardData = shard;
        onClickCallback = onClick;

        titleText.text = shardData.title;

        // LoreManager should be used to track read state externally
        bool isRead = LoreManager.Instance.IsShardRead(shardData.id);
        newTag.SetActive(!isRead);

        GetComponent<Button>().onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        onClickCallback?.Invoke(shardData);
    }
}
