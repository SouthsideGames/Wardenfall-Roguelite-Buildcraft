using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SigilsUI : MonoBehaviour
{
    private TextMeshProUGUI text;

    public void UpdateText(string _currencyString)
    {
        if(text == null)
           text = GetComponent<TextMeshProUGUI>();
           
        text.text = _currencyString;
    }
}
