using UnityEngine;
using TMPro;

public class Reward : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amountText;

    public void SetAmount(int amount)
    {
        if (amountText != null)
        {
            amountText.text = amount.ToString();
        }
    }
}
