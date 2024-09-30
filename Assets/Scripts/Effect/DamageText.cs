using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshPro damageText; 

    public void PlayAnimation(string _damage, bool _isCriticalHit)
    {
        damageText.text = _damage.ToString();   
        anim.Play("Animate");
        damageText.color = _isCriticalHit ? Color.yellow : Color.white;
    }

}
 