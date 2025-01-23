using System.Collections;
using UnityEngine;

public class PlasmaBeam : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer; // Line renderer for the beam
    [SerializeField] private LayerMask enemyMask; // Mask to detect enemies
    [SerializeField] private bool isVertical = true; // Whether the beam is vertical or horizontal

    private int damagePerTick;
    private float beamDuration;
    private float tickInterval = 0.1f;

    public void Configure(int damagePerTick, float beamDuration)
    {
        this.damagePerTick = damagePerTick;
        this.beamDuration = beamDuration;
    }

    private void Start()
    {
        InitializeBeam(); // Set up the beam direction
        Destroy(gameObject, beamDuration); // Destroy the plasma beam object after its duration
        StartCoroutine(DamageEnemies());
    }

    private void InitializeBeam()
    {
        // Set the beam start and end positions based on its orientation
        Vector3 startPosition = transform.position;
        Vector3 endPosition;

        if (isVertical)
        {
            // Vertical beam from top to bottom
            startPosition.y = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
            endPosition = new Vector3(startPosition.x, Camera.main.ScreenToWorldPoint(Vector3.zero).y, 0);
        }
        else
        {
            // Horizontal beam from left to right
            startPosition.x = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
            endPosition = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x, startPosition.y, 0);
        }

        // Configure the line renderer
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    private IEnumerator DamageEnemies()
    {
        while (beamDuration > 0)
        {
            // Perform a raycast to detect enemies
            Vector3 startPoint = lineRenderer.GetPosition(0);
            Vector3 endPoint = lineRenderer.GetPosition(1);
            Vector2 direction = (endPoint - startPoint).normalized;

            float length = Vector2.Distance(startPoint, endPoint);
            RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, direction, length, enemyMask);

            foreach (RaycastHit2D hit in hits)
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damagePerTick, false);
                }
            }

            yield return new WaitForSeconds(tickInterval);
            beamDuration -= tickInterval;
        }
    }
}
