using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude < 0.001)
            anim.Play("Character_Idle");
        else
            anim.Play("Character_Move");
    }
    
    public void SetTrigger() => anim.Play("Character_Dash");
}
