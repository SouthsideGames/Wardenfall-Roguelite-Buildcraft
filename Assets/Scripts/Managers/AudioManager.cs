using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour, IGameStateListener
{
    public static AudioManager Instance;

    [Header("ELEMENTS:")]
    [SerializeField] private AudioClip[] backgroundMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip stageCompleteMusic;
    [SerializeField] private AudioClip gameoverMusic;
    [SerializeField] private AudioClip uIMusic;

    [Header("SETTINGS:")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sFXSource;
    [SerializeField] private AudioSource uISource;

    public bool isSFXMuted{get; private set;}  
    public bool isMusicMuted{get; private set;}  

    private void Awake() 
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        SettingManager.onSFXStateChanged += SFXStateChangedCallback;
        SettingManager.onMusicStateChanged += MusicStateChangedCallback;
    }

    private void OnDestroy() 
    {
        SettingManager.onSFXStateChanged -= SFXStateChangedCallback;
        SettingManager.onMusicStateChanged -= MusicStateChangedCallback;
    }

  

    void Start() => PlayBackgroundMusic();

    private void Update() 
    {
       if(isMusicMuted)
          musicSource.mute = true;
       else
         musicSource.mute = false;

    }

    private void PlayBackgroundMusic()
    { 
        musicSource.clip = SelectRandomBackgroundMusic();
        musicSource.Play();
    }

    private AudioClip SelectRandomBackgroundMusic()
    {
        AudioClip randomClip = null;

        if(backgroundMusic.Length > 0)
        {
            int randomAudio = Random.Range(0, backgroundMusic.Length);    

            for (int i = 0; i < backgroundMusic.Length; i++)
            {
                if (i == randomAudio)
                {
                    randomClip = backgroundMusic[i];
                    break; 
                }
            }
        }

        return randomClip;
    }

    public void ChangeMusic(AudioClip _musicToPlay)
    {
        ResetMusicVolume();
        musicSource.clip = _musicToPlay;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip _audioToPlay)
    {
        if(!isSFXMuted)
           return;

        sFXSource.pitch = Random.Range(0.95f, 1.05f);
        sFXSource.PlayOneShot(_audioToPlay);
    }

    public void GameStateChangedCallback(GameState _gameState)
    {

        switch(_gameState)
        {
            case GameState.Menu:
                PlayBackgroundMusic();
                break;
            case GameState.Game:
                ChangeMusic(gameMusic);
                break;
            case GameState.GameOver:
                ChangeMusic(gameoverMusic);
                break;
            case GameState.StageCompleted:
                ChangeMusic(stageCompleteMusic);
                break;
            case GameState.WaveTransition:
                DecreaseMusicVolume();
                break;
            case GameState.Shop:
                DecreaseMusicVolume();
                break;
        }
    }

    private void MusicStateChangedCallback(bool _musicState) =>    isMusicMuted = !_musicState;
    private void SFXStateChangedCallback(bool _sfxState) => isSFXMuted =  !_sfxState;
    public void DecreaseMusicVolume() => musicSource.volume = musicSource.volume /5;
    public void ResetMusicVolume() => musicSource.volume = 0.4f;

}
