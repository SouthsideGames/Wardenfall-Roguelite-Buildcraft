using UnityEngine;

public class ProgressionUI : MonoBehaviour
{
    [SerializeField] private GameObject unlockTreePanel;

    public void OnOpenUnlockTree()
    {
        unlockTreePanel.SetActive(true);
        unlockTreePanel.GetComponent<UnlockTreeUI>()?.Refresh();
    }

    public void OnCloseUnlockTree()
    {
        unlockTreePanel.SetActive(false);
    }
}
