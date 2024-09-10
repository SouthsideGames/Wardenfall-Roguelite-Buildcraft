using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshPro damageText; 

    public void PlayAnimation(int _damage)
    {
        damageText.text = _damage.ToString();   
        anim.Play("Animate");
    }
}
