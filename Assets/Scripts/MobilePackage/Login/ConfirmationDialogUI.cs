using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ConfirmationDialogUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Action onConfirm;

    public void Initialize(string title, string message, Action onConfirm)
    {
        titleText.text = title;
        messageText.text = message;
        this.onConfirm = onConfirm;

        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
    }

    private void OnConfirm()
    {
        onConfirm?.Invoke();
        Destroy(gameObject);
    }

    private void OnCancel()
    {
        Destroy(gameObject);
    }
}
