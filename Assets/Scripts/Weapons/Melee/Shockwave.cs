using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [Header("SETTINGS")]
    public float growthRate = 2.0f;
    public float maxRadius = 5.0f; 
    public int damage = 10;   
    public LayerMask enemyMask;

    private CircleCollider2D shockwaveCollider;
    private Vector3 initialScale;

    private void Awake()
    {
        shockwaveCollider = GetComponent<CircleCollider2D>();
        initialScale = transform.localScale;
    }

    private void OnEnable()
    {
        StartCoroutine(ExpandShockwave());
    }

    private IEnumerator ExpandShockwave()
    {
        float currentRadius = shockwaveCollider.radius;

        while (currentRadius < maxRadius)
        {
            // Grow the shockwave's size
            currentRadius += growthRate * Time.deltaTime;
            shockwaveCollider.radius = currentRadius;
            transform.localScale = initialScale * (currentRadius / shockwaveCollider.radius);

            // Check for enemies within the shockwave’s radius
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, currentRadius, enemyMask);
            foreach (Collider2D enemyCollider in hitEnemies)
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, false);
                }
            }

            yield return null;
        }

        Destroy(gameObject); // Destroy shockwave once max size is reached
    }
}
