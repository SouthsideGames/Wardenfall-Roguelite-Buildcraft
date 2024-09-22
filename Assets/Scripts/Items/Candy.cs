using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Candy : Item
{
     [Header("ACTIONS:")]
    public static Action<Candy> onCollected;
    
    protected override void Collected()
    {
        onCollected?.Invoke(this);
    }   
}
