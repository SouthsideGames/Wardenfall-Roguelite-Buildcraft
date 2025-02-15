using System.Collections;
using SouthsideGames.DailyMissions;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
   [Header("SETTINGS")]
    [SerializeField] private LayerMask enemyMask;

    private float growthRate = 2.0f;
    private float maxRadius = 5.0f; 
    private int damage = 10;        
    private CircleCollider2D shockwaveCollider;
    private Vector3 initialScale;

    private void Awake()
    {
        shockwaveCollider = GetComponent<CircleCollider2D>();
        initialScale = transform.localScale;
    }

    public void Initialize(LayerMask enemyMask, int damage, float growthRate, float maxRadius)
    {
        this.enemyMask = enemyMask;
        this.damage = damage;
        this.growthRate = growthRate;
        this.maxRadius = maxRadius;
    }

    private void OnEnable() => StartCoroutine(ExpandShockwave());

    private IEnumerator ExpandShockwave()
    {
        float currentRadius = shockwaveCollider.radius;

        while (currentRadius < maxRadius)
        {
            currentRadius += growthRate * Time.deltaTime;
            shockwaveCollider.radius = currentRadius;
            transform.localScale = initialScale * (currentRadius / shockwaveCollider.radius);

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, currentRadius, enemyMask);
            foreach (Collider2D enemyCollider in hitEnemies)
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, false);
                    MissionManager.Increment(MissionType.shockwaveSpecialist, damage);
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
