using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private static readonly Queue<DamageText> Pool = new Queue<DamageText>();
    private static readonly int MaxPoolSize = SystemInfo.deviceType == DeviceType.Handheld ? 10 : 20;
    private static readonly bool useLowQualityParticles = SystemInfo.deviceType == DeviceType.Handheld;

    public static DamageText Get()
    {
        if (Pool.Count > 0)
            return Pool.Dequeue();
        return null;
    }

    public void ReturnToPool()
    {
        if (Pool.Count < MaxPoolSize)
        {
            Pool.Enqueue(this);
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
    [Header("Elements")]
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshPro damageText; 

    public void PlayAnimation(string _damage, bool _isCriticalHit)
    {
        damageText.text = _damage.ToString();   
        anim.Play("Animate");
        damageText.color = _isCriticalHit ? Color.yellow : Color.white;
        
        // Add haptic feedback
        if (_isCriticalHit)
            HapticFeedbackUI.TriggerMedium();
        else
            HapticFeedbackUI.Trigger();
            
        // Spawn hit effect particle
        EffectManager.Instance.SpawnHitEffect(transform.position);
    }

}