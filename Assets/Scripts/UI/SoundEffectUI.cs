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
    }

    public void PlaySFX()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => AudioManager.Instance.PlaySFX(clipToPlay));
    
    }
}
