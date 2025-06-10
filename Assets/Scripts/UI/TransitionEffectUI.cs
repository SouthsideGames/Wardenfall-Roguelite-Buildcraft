using UnityEngine;

public class TransitionSoundEffectUI : MonoBehaviour
{
    [Header("Sound Effects")]
    [SerializeField] private AudioClip tvOnSound;
    [SerializeField] private AudioClip tvOffSound;
    [SerializeField] private AudioClip tvPopSound;


    public void PlayTVOnSFX() => PlaySFX(tvOnSound);
    public void PlayTVOffSFX() => PlaySFX(tvOffSound);
    public void PlayTVPopSFX() => PlaySFX(tvPopSound);

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
           AudioManager.Instance.PlaySFX(clip);
    }
}
