using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   
using TMPro;
using Tabsil.Sijil;
using UnityEngine.Networking;

public class SettingManager : MonoBehaviour, IWantToBeSaved
{
    [Header("ELEMENTS:")]
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button privacyPolicyButton;
    [SerializeField] private Button askButton;


    [Header("SETTINGS:")]
    [SerializeField] private Color onColor;
    [SerializeField] private Color offColor;
    [SerializeField] private String privacyPolicyURL;

    private bool sfxState;
    private bool musicState;

    private const string sfxKey = "SFX";
    private const string musicKey = "Music";

    private void Awake() 
    {
        sfxButton.onClick.RemoveAllListeners();
        sfxButton.onClick.AddListener(SFXButtonCallback);    

        musicButton.onClick.RemoveAllListeners();
        musicButton.onClick.AddListener(MusicButtonCallback);    

        privacyPolicyButton.onClick.RemoveAllListeners();
        privacyPolicyButton.onClick.AddListener(PrivacyPolicyButtonCallback);
        
        askButton.onClick.RemoveAllListeners();
        askButton.onClick.AddListener(AskButtonCallback);
    }

    private void AskButtonCallback()
    {
        string email = "southsidegames2021@gmail.com";
        string subject = EscapeURL("Help");
        string body = EscapeURL("hey ! I need help with this...");

        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    private string EscapeURL(string _s) => UnityWebRequest.EscapeURL(_s).Replace("+", "%20");
    private void SFXButtonCallback()
    {
        sfxState = !sfxState;

        UpdateSFXVisuals();

        Save();

        //Trigger an action
    }

    
    private void MusicButtonCallback()
    {
        musicState = !musicState;

        UpdateMusicVisuals();

        Save();

        //Trigger an action
    }

    private void UpdateSFXVisuals()
    {
        if(sfxState)
        {
            sfxButton.image.color = onColor;
            sfxButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
        }
        else 
        {
            sfxButton.image.color = offColor;
            sfxButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
        }
    }

    private void UpdateMusicVisuals()
    {
        if(sfxState)
        {
            musicButton.image.color = onColor;
            musicButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
        }
        else 
        {
            musicButton.image.color = offColor;
            musicButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
        }
    }

    private void PrivacyPolicyButtonCallback()
    {
        Application.OpenURL(privacyPolicyURL);
    }

    public void Load()
    {
        sfxState = true;
        musicState = true;

        if(Sijil.TryLoad(this, sfxKey, out object sfxStateObject))
           sfxState = (bool)sfxStateObject;

        if(Sijil.TryLoad(this, musicKey, out object musicStateObject))
           musicState = (bool)musicStateObject;

        UpdateMusicVisuals();
        UpdateSFXVisuals();

    }

    public void Save()
    {
        Sijil.Save(this, sfxKey, sfxState);
        Sijil.Save(this, musicKey, musicState);
    }
}
