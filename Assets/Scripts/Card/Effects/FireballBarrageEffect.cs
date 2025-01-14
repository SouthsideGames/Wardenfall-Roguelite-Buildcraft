using UnityEngine;


public class FireballBarrageEffect : ICardEffect
{
     private GameObject fireballPrefab;
    private int fireballCount;
    private float spawnRadius;
    private int baseDamage;
    private bool isCriticalHit;

    public FireballBarrageEffect(GameObject fireballPrefab, int fireballCount, float spawnRadius, int baseDamage, bool isCriticalHit = false)
    {
        this.fireballPrefab = fireballPrefab;
        this.fireballCount = fireballCount;
        this.spawnRadius = spawnRadius;
        this.baseDamage = baseDamage;
        this.isCriticalHit = isCriticalHit;
    }

    public void Activate(float duration)
    {
        for (int i = 0; i < fireballCount; i++)
        {
            // Generate a random position within the spawn radius
            Vector2 spawnPosition = (Vector2)CharacterManager.Instance.transform.position + Random.insideUnitCircle * spawnRadius;

            // Generate a random target position for each fireball
            Vector2 targetPosition = spawnPosition + Random.insideUnitCircle.normalized;

            // Instantiate and configure the fireball
            GameObject fireball = Object.Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);
            Vector2 direction = (targetPosition - spawnPosition).normalized;

            fireball.GetComponent<Fireball>().Launch(baseDamage, direction, isCriticalHit);
        }

        Debug.Log($"Fireball Barrage activated: {fireballCount} fireballs launched.");
    }

    public void Disable()
    {
        Debug.Log("Fireball Barrage does not require a disable phase.");
    }
}
