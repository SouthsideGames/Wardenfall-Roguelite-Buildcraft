using System;
using UnityEngine;
using UnityEngine.UI;   
using TMPro;
using SouthsideGames.SaveManager;
using UnityEngine.Networking;

public class SettingManager : MonoBehaviour, IWantToBeSaved
{
    public static SettingManager Instance;

    public static Action<bool> onSFXStateChanged;
    public static Action<bool> onMusicStateChanged;
    public static Action<bool> onVibrateStateChanged;

    [Header("ELEMENTS:")]
    [SerializeField] private Sprite offImage;
    [SerializeField] private Sprite onImage;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button vibrateButton;
    [SerializeField] private Button privacyPolicyButton;
    [SerializeField] private Button askButton;
    [SerializeField] private Button creditsButton;

    [Header("DIFFICULTY SETTINGS:")]
    [SerializeField] private Button difficultyButton;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private string[] difficultyLevels = { "Easy", "Normal", "Hard", "Expert" };
    [SerializeField] private int initialDifficultyIndex = 1;


    [Header("SETTINGS:")]
    [SerializeField] private String privacyPolicyURL;
    [SerializeField] private GameObject creditsPanel;

    private bool sfxState;
    private bool musicState;
    private bool vibrateState;
    [HideInInspector] public int currentDifficultyIndex;

    private const string sfxKey = "SFX";
    private const string musicKey = "Music";
    private const string vibrateKey = "Vibrate";
    private const string difficultyKey = "Difficulty";

    private void Awake() 
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        sfxButton.onClick.RemoveAllListeners();
        sfxButton.onClick.AddListener(SFXButtonCallback);    

        musicButton.onClick.RemoveAllListeners();
        musicButton.onClick.AddListener(MusicButtonCallback);    
        
        vibrateButton.onClick.RemoveAllListeners();
        vibrateButton.onClick.AddListener(VibrateButtonCallback);    

        privacyPolicyButton.onClick.RemoveAllListeners();
        privacyPolicyButton.onClick.AddListener(PrivacyPolicyButtonCallback);
        
        askButton.onClick.RemoveAllListeners();
        askButton.onClick.AddListener(AskButtonCallback);

        creditsButton.onClick.RemoveAllListeners();
        creditsButton.onClick.AddListener(CreditsButtonCallback);

        difficultyButton.onClick.RemoveAllListeners();
        difficultyButton.onClick.AddListener(DifficultyButtonCallback);
    }

    private void Start() 
    {
        HideCreditsPanel();

        currentDifficultyIndex = initialDifficultyIndex;

        onSFXStateChanged?.Invoke(sfxState);
        onMusicStateChanged?.Invoke(musicState);    
        onVibrateStateChanged?.Invoke(vibrateState);
        UpdateDifficultyVisuals();
    }

    private void AskButtonCallback()
    {
        string email = "southsidegames2021@gmail.com";
        string subject = EscapeURL("Help");
        string body = EscapeURL("hey ! I need help with this...");

        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    private void SFXButtonCallback()
    {
        sfxState = !sfxState;

        UpdateSFXVisuals();

        Save();

        //Trigger an action
        onSFXStateChanged?.Invoke(sfxState);
    }

    
    private void MusicButtonCallback()
    {
        musicState = !musicState;

        UpdateMusicVisuals();

        Save();

        //Trigger an action
        onMusicStateChanged?.Invoke(musicState);    
    }

    private void VibrateButtonCallback()
    {
        vibrateState = !vibrateState;

        UpdateVibrateVisuals();

        Save();

        //Trigger an action
        onVibrateStateChanged?.Invoke(vibrateState);
    }


    private void UpdateSFXVisuals()
    {
        if(sfxState)
        {
            musicButton.image.sprite = onImage;
            sfxButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
        }
        else 
        {
            musicButton.image.sprite = offImage;
            sfxButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
        }
    }

    private void UpdateMusicVisuals()
    {
        if(musicState)
        {
            musicButton.image.sprite = onImage;
            musicButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
        }
        else 
        {
            musicButton.image.sprite = offImage;
            musicButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
        }
    }

    private void UpdateVibrateVisuals()
    {
        if(vibrateState)
        {
            musicButton.image.sprite = onImage;
            vibrateButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
        }
        else 
        {
            musicButton.image.sprite = offImage;
            vibrateButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
        }
    }

    private void DifficultyButtonCallback()
    {
        currentDifficultyIndex = (currentDifficultyIndex + 1) % difficultyLevels.Length;

        UpdateDifficultyVisuals();

        Save();
    }

    

    private void CreditsButtonCallback() => creditsPanel.SetActive(true);
    public void HideCreditsPanel() => creditsPanel.SetActive(false);
    private string EscapeURL(string _s) => UnityWebRequest.EscapeURL(_s).Replace("+", "%20");
    private void PrivacyPolicyButtonCallback() => Application.OpenURL(privacyPolicyURL);
    private void UpdateDifficultyVisuals() => difficultyText.text = difficultyLevels[currentDifficultyIndex];

    public void Load()
    {
        sfxState = true;
        musicState = true;
        vibrateState = true;

        if(SaveManager.TryLoad(this, sfxKey, out object sfxStateObject))
            sfxState = (bool)sfxStateObject;

        if(SaveManager.TryLoad(this, musicKey, out object musicStateObject))
            musicState = (bool)musicStateObject;

        if(SaveManager.TryLoad(this, vibrateKey, out object vibrateStateObject))
            vibrateState = (bool)vibrateStateObject;

        if (SaveManager.TryLoad(this, difficultyKey, out object difficultyObject))
            currentDifficultyIndex = (int)difficultyObject;


        UpdateMusicVisuals();
        UpdateSFXVisuals();
        UpdateVibrateVisuals();
        UpdateDifficultyVisuals();
    }

    public void Save()
    {
        SaveManager.Save(this, sfxKey, sfxState);
        SaveManager.Save(this, musicKey, musicState);
        SaveManager.Save(this, vibrateKey, vibrateState);
        SaveManager.Save(this, difficultyKey, currentDifficultyIndex);
    }
}
