using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectUI : MonoBehaviour
{
    [SerializeField] private AudioClip clipToPlay;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlaySFX);
    }

    private void PlaySFX() => AudioManager.Instance.PlaySFX(clipToPlay);
}
