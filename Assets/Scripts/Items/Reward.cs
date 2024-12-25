using UnityEngine;
using TMPro;

public class Reward : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amountText;

    public void SetAmount(int amount, bool isCard)
    {
        if (amountText != null)
        {
            amountText.gameObject.SetActive(!isCard);
            if (!isCard)
            {
                amountText.text = amount.ToString();
            }
        }
    }
}
