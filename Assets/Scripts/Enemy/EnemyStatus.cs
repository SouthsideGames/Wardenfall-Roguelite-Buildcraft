using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    private Enemy enemy;
    private bool isDraining = false;

    [Header("DRAIN SETTINGS")]
    [SerializeField] private int drainAmount = 1;     
    [SerializeField] private float drainInterval = 1.0f;    

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void ApplyLifeDrain(float duration)
    {
        if (!isDraining)
        {
            isDraining = true;
            StartCoroutine(DrainHealth(duration));
        }
    }

    private IEnumerator DrainHealth(float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            if (isDraining)
            {
                enemy.TakeDamage(drainAmount, false);
                elapsedTime += drainInterval;
                yield return new WaitForSeconds(drainInterval);
            }
            else
            {
                yield break;
            }
        }

        isDraining = false;
    }

    public void StopLifeDrain()
    {
        isDraining = false;
    }
}
