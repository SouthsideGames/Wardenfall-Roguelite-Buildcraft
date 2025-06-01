
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour, IGameStateListener
{
    public static AudioManager Instance;

    [Header("ELEMENTS:")]
    [SerializeField] private AudioClip[] backgroundMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip stageCompleteMusic;
    [SerializeField] private AudioClip gameoverMusic;

    [Header("SOURCES:")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sFXSource;
    [SerializeField] private AudioSource crowdNoiseSource;
    [SerializeField] private AudioSource crowdReactionSource;

    [Header("Crowd Reactions")]
    [SerializeField] private List<AudioClip> crowdCheers;
    [SerializeField] private List<AudioClip> crowdGasps;
    [SerializeField] private List<AudioClip> crowdRoars;
    [SerializeField] private List<AudioClip> crowdBoos;
    [SerializeField] private List<AudioClip> crowdLaughs;
    [SerializeField] private List<AudioClip> crowdWhistles;
    [SerializeField] private List<AudioClip> crowdOuchDrops;
    [SerializeField] private AudioClip commericalClip;  
    [SerializeField] private AudioClip stageOverClip;  

    public bool isSFXMuted { get; private set; }
    public bool isMusicMuted { get; private set; }

    private void Awake()
    {
        if (Instance == null)
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
        musicSource.mute = isMusicMuted;
        crowdNoiseSource.mute = isSFXMuted;
        crowdReactionSource.mute = isSFXMuted;
    }

    private void PlayBackgroundMusic()
    {
        musicSource.clip = SelectRandomBackgroundMusic();
        musicSource.Play();
    }

    private AudioClip SelectRandomBackgroundMusic()
    {
        if (backgroundMusic.Length > 0)
        {
            int index = Random.Range(0, backgroundMusic.Length);
            return backgroundMusic[index];
        }
        return null;
    }

    public void ChangeMusic(AudioClip _musicToPlay)
    {
        ResetMusicVolume();
        musicSource.clip = _musicToPlay;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip _audioToPlay)
    {
        if (isSFXMuted) return;

        sFXSource.pitch = Random.Range(0.95f, 1.05f);
        sFXSource.PlayOneShot(_audioToPlay);
    }

    public void PlayCrowdAmbience()
    {
        if (isSFXMuted || crowdNoiseSource.isPlaying) return;

        crowdNoiseSource.loop = true;
        crowdNoiseSource.Play();
    }

    
    public void StopAmbientLoop()
    {
        if (crowdNoiseSource.isPlaying)
            crowdNoiseSource.Stop();
    }

    public void PlayCrowdReaction(CrowdReactionType type)
    {
        if (isSFXMuted || crowdReactionSource == null) return;

        AudioClip clip = GetRandomReaction(type);
        if (clip != null)
        {
            crowdReactionSource.pitch = Random.Range(0.95f, 1.05f);
            crowdReactionSource.PlayOneShot(clip);
        }
    }

    private AudioClip GetRandomReaction(CrowdReactionType type)
    {
        switch (type)
        {
            case CrowdReactionType.Cheer: return GetRandomFromList(crowdCheers);
            case CrowdReactionType.Gasp: return GetRandomFromList(crowdGasps);
            case CrowdReactionType.Roar: return GetRandomFromList(crowdRoars);
            case CrowdReactionType.Boo: return GetRandomFromList(crowdBoos);
            case CrowdReactionType.Laugh: return GetRandomFromList(crowdLaughs);
            case CrowdReactionType.Whistle: return GetRandomFromList(crowdWhistles);
            case CrowdReactionType.Ouch: return GetRandomFromList(crowdOuchDrops);
        }
        return null;
    }

    private AudioClip GetRandomFromList(List<AudioClip> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }

    private void MusicStateChangedCallback(bool _musicState) => isMusicMuted = !_musicState;

    private void SFXStateChangedCallback(bool _sfxState)
    {
        isSFXMuted = !_sfxState;

        crowdReactionSource.mute = isSFXMuted;
        crowdNoiseSource.mute = isSFXMuted;
    }

    public void GameStateChangedCallback(GameState _gameState)
    {
        switch (_gameState)
        {
            case GameState.Menu:
                PlayBackgroundMusic();
                break;
            case GameState.Game:
                ChangeMusic(gameMusic);
                break;
            case GameState.GameOver:
                PlaySFX(stageOverClip);
                Invoke("ChangeMusic(gameoverMusic)", 3f);
                break;
            case GameState.StageCompleted:
                PlaySFX(stageOverClip);
                ChangeMusic(stageCompleteMusic);
                break;
            case GameState.WaveTransition:
            case GameState.Shop:
                PlaySFX(commericalClip);
                DecreaseMusicVolume();
                break;
        }
    }

    public void DecreaseMusicVolume() => musicSource.volume = musicSource.volume / 5;
    public void ResetMusicVolume() => musicSource.volume = 0.4f;
}


