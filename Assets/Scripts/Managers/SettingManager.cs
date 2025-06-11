using System;
using UnityEngine;
using UnityEngine.UI;   
using TMPro;
using SouthsideGames.SaveManager;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingManager : MonoBehaviour, IWantToBeSaved
{
    public static SettingManager Instance;

    public static Action<bool> onSFXStateChanged;
    public static Action<bool> onMusicStateChanged;
    public static Action<bool> onVibrateStateChanged;
    public static Action<bool> onColorblindSupportChanged;
    public static Action<bool> onJoystickPositionChanged;

    [Header("ELEMENTS:")]
    [SerializeField] private Sprite offImage;
    [SerializeField] private Sprite onImage;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button vibrateButton;
    [SerializeField] private Button privacyPolicyButton;
    [SerializeField] private Button askButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button postProcessingButton;
    [SerializeField] private Volume postProcessingVolume;


    [Header("SETTINGS:")]
    [SerializeField] private String privacyPolicyURL;
    [SerializeField] private GameObject creditsPanel;

    [Header("Reset")]
    [SerializeField] private GameObject resetConfirmationPanel;

    [Header("ACCESSIBILITY")]
    [SerializeField] private Button colorblindSupportButton;

    public bool sfxState;
    public bool musicState;
    public bool vibrateState;
    public bool colorblindSupport;
    public bool joystickOnRight; // true = right side, false = left side

    private const string sfxKey = "SFX";
    private const string musicKey = "Music";
    private const string vibrateKey = "Vibrate";
    private const string colorblindKey = "ColorblindSupport";
    private const string joystickPositionKey = "JoystickPosition";
    private const string postProcessingKey = "PostProcessing";
    public bool postProcessingEnabled = true;

    private float lastButtonPressTime;
    private const float buttonCooldown = 0.5f;

    private void Awake()
    {
        if (Instance == null)
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

        colorblindSupportButton.onClick.RemoveAllListeners();
        colorblindSupportButton.onClick.AddListener(ColorblindSupportButtonCallback);
        
        postProcessingButton.onClick.RemoveAllListeners();
        postProcessingButton.onClick.AddListener(PostProcessingButtonCallback);

    }

    private void Start() 
    {
        HideCreditsPanel();

        onSFXStateChanged?.Invoke(sfxState);
        onMusicStateChanged?.Invoke(musicState);    
        onVibrateStateChanged?.Invoke(vibrateState);
        onColorblindSupportChanged?.Invoke(colorblindSupport);
        onJoystickPositionChanged?.Invoke(joystickOnRight);
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
        if (Time.time - lastButtonPressTime < buttonCooldown) return;
        lastButtonPressTime = Time.time;

        sfxState = !sfxState;

        UpdateSFXVisuals();

        Save();

        onSFXStateChanged?.Invoke(sfxState);
    }


    private void MusicButtonCallback()
    {
        if (Time.time - lastButtonPressTime < buttonCooldown) return;
        lastButtonPressTime = Time.time;

        musicState = !musicState;

        UpdateMusicVisuals();

        Save();

        onMusicStateChanged?.Invoke(musicState);    
    }

    private void VibrateButtonCallback()
    {
        vibrateState = !vibrateState;

        UpdateVibrateVisuals();

        Save();

        onVibrateStateChanged?.Invoke(vibrateState);
    }

    private void ColorblindSupportButtonCallback()
    {
        if (Time.time - lastButtonPressTime < buttonCooldown) return;
        lastButtonPressTime = Time.time;

        colorblindSupport = !colorblindSupport;

        UpdateColorblindSupportVisuals();

        Save();

        onColorblindSupportChanged?.Invoke(colorblindSupport);
    }

    private void UpdateSFXVisuals()
    {
        if(sfxState)
        {
            sfxButton.image.sprite = onImage;
            sfxButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
        }
        else 
        {
            sfxButton.image.sprite = offImage;
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
            vibrateButton.image.sprite = onImage;
            vibrateButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
        }
        else 
        {
            vibrateButton.image.sprite = offImage;
            vibrateButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
        }
    }

    private void UpdateColorblindSupportVisuals()
    {
        if(colorblindSupport)
        {
            colorblindSupportButton.image.sprite = onImage;
            colorblindSupportButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
        }
        else 
        {
            colorblindSupportButton.image.sprite = offImage;
            colorblindSupportButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
        }
    }

    private void PostProcessingButtonCallback()
    {
        if (Time.time - lastButtonPressTime < buttonCooldown) return;
        lastButtonPressTime = Time.time;

        postProcessingEnabled = !postProcessingEnabled;

        // Apply to the main camera
        Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing = postProcessingEnabled;

        UpdatePostProcessingVisuals();
        postProcessingVolume.enabled = postProcessingEnabled; // Optional if you use Volume blending
        Save();
    }


    private void UpdatePostProcessingVisuals()
    {
        if (postProcessingEnabled)
        {
            postProcessingButton.image.sprite = onImage;
            postProcessingButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
        }
        else
        {
            postProcessingButton.image.sprite = offImage;
            postProcessingButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
        }
    }


    public void OpenResetConfirmation()
    {
        resetConfirmationPanel.SetActive(true);
    }

    public void ConfirmResetButtonCallback()
    {
        UserManager.Instance.ClearAllData();
    }

    public void CancelResetButtonCallback()
    {
        resetConfirmationPanel.SetActive(false);
    }


    private void CreditsButtonCallback() => creditsPanel.SetActive(true);
    public void HideCreditsPanel() => creditsPanel.SetActive(false);
    private string EscapeURL(string _s) => UnityWebRequest.EscapeURL(_s).Replace("+", "%20");
    private void PrivacyPolicyButtonCallback() => Application.OpenURL(privacyPolicyURL);

    public void Load()
    {
        sfxState = true;
        musicState = true;
        vibrateState = true;
        colorblindSupport = false;
        joystickOnRight = false; 

        if (SaveManager.TryLoad(this, sfxKey, out object sfxStateObject))
            sfxState = (bool)sfxStateObject;

        if (SaveManager.TryLoad(this, musicKey, out object musicStateObject))
            musicState = (bool)musicStateObject;

        if (SaveManager.TryLoad(this, vibrateKey, out object vibrateStateObject))
            vibrateState = (bool)vibrateStateObject;

        if (SaveManager.TryLoad(this, postProcessingKey, out object postProcessingObject))
            postProcessingEnabled = (bool)postProcessingObject;

        postProcessingVolume.enabled = postProcessingEnabled;
        UpdatePostProcessingVisuals();


        // Don't load colorblind setting - always start with false
        // if (SaveManager.TryLoad(this, colorblindKey, out object colorblindStateObject))
        //     colorblindSupport = (bool)colorblindStateObject;

        if (SaveManager.TryLoad(this, joystickPositionKey, out object joystickPositionObject))
            joystickOnRight = (bool)joystickPositionObject;

        UpdateMusicVisuals();
        UpdateSFXVisuals();
        UpdateVibrateVisuals();
        UpdateColorblindSupportVisuals();
 
         if(!SaveManager.TryLoad(this, sfxKey, out _) || 
           !SaveManager.TryLoad(this, musicKey, out _) || 
           !SaveManager.TryLoad(this, vibrateKey, out _) ||
           !SaveManager.TryLoad(this, joystickPositionKey, out _))
        {
            Save();
        }
    }

    public void Save()
    {
        SaveManager.Save(this, sfxKey, sfxState);
        SaveManager.Save(this, musicKey, musicState);
        SaveManager.Save(this, vibrateKey, vibrateState);
        SaveManager.Save(this, colorblindKey, colorblindSupport);
        SaveManager.Save(this, joystickPositionKey, joystickOnRight);
        SaveManager.Save(this, postProcessingKey, postProcessingEnabled);
    }
}