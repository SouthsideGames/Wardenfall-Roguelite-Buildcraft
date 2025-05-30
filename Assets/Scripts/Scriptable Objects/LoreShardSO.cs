using UnityEngine;

[CreateAssetMenu(fileName = "Lore Shard Data", menuName = "Scriptable Objects/New Lore Shard Data", order = 13)]
public class LoreShardSO : MonoBehaviour
{
    public int id;
    public string title;
    [TextArea(4, 20)] public string body;
}
