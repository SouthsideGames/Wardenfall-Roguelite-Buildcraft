using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshPro damageText; 

    public void PlayAnimation(int _damage, bool _isCriticalHit)
    {
        damageText.text = _damage.ToString();   
        damageText.color = _isCriticalHit ? Color.yellow : Color.white;
        anim.Play("Animate");
    }
}
 