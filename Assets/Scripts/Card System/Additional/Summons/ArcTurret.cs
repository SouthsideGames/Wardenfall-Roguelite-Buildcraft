using System.Collections;
using UnityEngine;

public class ArcTurret : MonoBehaviour
{
    [SerializeField] private float chainInterval = 1.5f;
    [SerializeField] private float chainRange = 5f;
    [SerializeField] private int chainCount = 3;
    [SerializeField] private int damage = 15;
    [SerializeField] private GameObject lightningVFXPrefab;

    private float lifetime;

    public void Initialize(float duration)
    {
        lifetime = duration;
        StartCoroutine(ChainRoutine());
        Destroy(gameObject, lifetime);
    }

    private IEnumerator ChainRoutine()
    {
        float elapsed = 0f;

        while (elapsed < lifetime)
        {
            FireChainLightning();
            yield return new WaitForSeconds(chainInterval);
            elapsed += chainInterval;
        }
    }

    private void FireChainLightning()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Transform origin = transform;
        int hits = 0;

        while (hits < chainCount)
        {
            Enemy closest = null;
            float closestDistance = Mathf.Infinity;

            foreach (Enemy enemy in enemies)
            {
                if (enemy == null) continue;
                float dist = Vector2.Distance(origin.position, enemy.transform.position);
                if (dist < closestDistance && dist <= chainRange)
                {
                    closest = enemy;
                    closestDistance = dist;
                }
            }

            if (closest == null) break;

            closest.TakeDamage(damage);
            CreateLightningEffect(origin.position, closest.transform.position);

            origin = closest.transform;
            hits++;
        }
    }

    private void CreateLightningEffect(Vector3 from, Vector3 to)
    {
        if (lightningVFXPrefab != null)
        {
            GameObject effect = Instantiate(lightningVFXPrefab, from, Quaternion.identity);
            LineRenderer lr = effect.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.SetPosition(0, from);
                lr.SetPosition(1, to);
            }
            Destroy(effect, 0.5f);
        }
    }
}
