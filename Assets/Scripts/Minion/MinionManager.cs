using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : MonoBehaviour
{
     [Header("SETTINGS:")]
    [SerializeField] private float lifetime;
    [SerializeField] private int damage;

    private void Start()
    {
        StartCoroutine(LifetimeCountdown());
    }

    public void InitializeMinion(float _lifetime, int _damage)
    {
        lifetime = _lifetime;
        damage = _damage;
    }

    public int GetDamage()
    {
        return damage;
    }

    private IEnumerator LifetimeCountdown()
    {
        yield return new WaitForSeconds(lifetime);
        DestroyMinion();
    }

    private void DestroyMinion()
    {
        Destroy(gameObject);
    }
}
