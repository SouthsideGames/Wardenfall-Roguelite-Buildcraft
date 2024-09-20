using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chest : Item
{
    [Header("Actions")]
    public static Action<Chest> onCollected;

    [Header("Settings")]
    [SerializeField] private Animator anim;

    protected override void Collected()
    {
        onCollected?.Invoke(this);  
        anim.SetTrigger("open");
    }   

    public void AnimationComplete() => Destroy(gameObject);    

 
}
