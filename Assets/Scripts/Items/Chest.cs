using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chest : Item
{
    [Header("ACTIONS:")]
    public static Action<Chest> OnCollected;

    [Header("SETTINGS:")]
    [SerializeField] private Animator anim;

    protected override void Collected()
    {
        OnCollected?.Invoke(this);  
        anim.SetTrigger("open");
    }   

    public void AnimationComplete() => Destroy(gameObject);    

 
}
