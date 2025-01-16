using System.Collections;
using UnityEngine;

public class LightningBolt : MonoBehaviour
{
    [Tooltip("The max number of jumps the lightning can make.")]
    [SerializeField] private int maxJumps = 5;

    [Tooltip("The delay between lightning jumps.")]
    [SerializeField] private float jumpDelay = 0.1f;

    [Tooltip("The damage dealt by the lightning.")]
    [SerializeField] private int damage = 100;

    [Tooltip("The layer mask for detecting enemies.")]
    [SerializeField] private LayerMask enemyMask;

    private LineRenderer lineRenderer;
    private int damageMultiplier;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Activate(Vector2 startPosition, int damageMultiplier)
    {
        this.damageMultiplier = damageMultiplier;
        StartCoroutine(ChainLightning(startPosition));
    }

    private IEnumerator ChainLightning(Vector2 startPosition)
    {
        Vector2 currentPosition = startPosition;
        lineRenderer.positionCount = 0;

        for (int i = 0; i < maxJumps; i++)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(currentPosition, 5f, enemyMask);
            if (hitEnemies.Length == 0)
                break;

            Enemy target = hitEnemies[Random.Range(0, hitEnemies.Length)].GetComponent<Enemy>();
            if (target == null)
                break;

            Vector2 targetPosition = target.transform.position;

            lineRenderer.positionCount += 2;
            lineRenderer.SetPosition(lineRenderer.positionCount - 2, currentPosition);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, targetPosition);

            target.TakeDamage(damage * damageMultiplier / 100, false);

            currentPosition = targetPosition;

            yield return new WaitForSeconds(jumpDelay);
        }

        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}
