using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chest : Item
{
    [Header("ACTIONS:")]
    public static Action<Chest> OnCollected;

    [Header("SETTINGS:")]
    [SerializeField] private AudioClip collectSFX;
    private Animator anim;

    private void Awake() => anim = GetComponent<Animator>();    

    protected override void Collected()
    {
        AudioManager.Instance.PlaySFX(collectSFX);
        OnCollected?.Invoke(this);  
        anim.SetTrigger("open");
    }   

    public void AnimationComplete() => Destroy(gameObject);    

 
}
