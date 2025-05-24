using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UsernamePopupUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Button confirmButton;
    
    private TouchScreenKeyboard keyboard;

    private void Start()
    {
        
        usernameInput.onSelect.AddListener(OnInputSelect);
        
        if (UserManager.Instance.IsFirstTimePlayer())
            Show();
        else
            GameManager.Instance.StartMainMenu();
    }

    private void OnInputSelect(string value)
    {
        if (TouchScreenKeyboard.isSupported)
            keyboard = TouchScreenKeyboard.Open(usernameInput.text, TouchScreenKeyboardType.Default);
    }

    private void Update()
    {
        if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            usernameInput.text = keyboard.text;
            keyboard = null;
        }
    }

    public void Show() => usernameInput.text = "";

    public void OnConfirmUsername()
    {
        if (string.IsNullOrEmpty(usernameInput.text))
            return;

        UserManager.Instance.SetUsername(usernameInput.text);
        ProgressionManager.Instance.progressionMenuUI.UpdateInfo();
        GameManager.Instance.StartMainMenu();
    }
}
