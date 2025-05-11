using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshPro damageText; 

    private static readonly Queue<DamageText> Pool = new Queue<DamageText>();
    private static readonly int MaxPoolSize = 20;


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
    }

    public void PlayAnimation(string _damage, bool _isCriticalHit)
    {
        damageText.text = _damage.ToString();   
        anim.Play("Animate");
        damageText.color = _isCriticalHit ? Color.yellow : Color.white;

    }

}