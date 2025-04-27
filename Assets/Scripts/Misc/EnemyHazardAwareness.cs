using UnityEngine;

public class EnemyHazardAwareness : MonoBehaviour
{
    private EnemyMovement movement;
    private Enemy enemy;
    private float hazardCheckRadius = 3f;
    private float hazardAvoidanceForce = 5f;

    private void Start()
    {
        movement = GetComponent<EnemyMovement>();
        enemy = GetComponent<Enemy>();
    }

    private void FixedUpdate()
    {
        CheckForHazards();
    }

    private void CheckForHazards()
    {
        Collider2D[] hazards = Physics2D.OverlapCircleAll(transform.position, hazardCheckRadius);
        Vector2 avoidanceDirection = Vector2.zero;

        foreach (Collider2D hazard in hazards)
        {
            EnvironmentalHazard hazardComponent = hazard.GetComponent<EnvironmentalHazard>();
            if (hazardComponent != null)
            {
                // Don't avoid healing zones
                if (hazardComponent is HealZone && enemy.CurrentHealth < enemy.MaxHealth)
                {
                    continue;
                }

                Vector2 directionFromHazard = (transform.position - hazard.transform.position).normalized;
                float distance = Vector2.Distance(transform.position, hazard.transform.position);
                avoidanceDirection += directionFromHazard * (hazardCheckRadius - distance) / hazardCheckRadius;
            }
        }

        if (avoidanceDirection != Vector2.zero && movement != null)
        {
            movement.AddForce(avoidanceDirection.normalized * hazardAvoidanceForce);
        }
    }
}