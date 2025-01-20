using UnityEngine;


public class FireballBarrageEffect : ICardEffect
{
    private GameObject fireballPrefab;
    private int fireballCount = 5;
    private float spawnRadius = 10;
    private int damage;

    public FireballBarrageEffect(GameObject fireballPrefab, CardSO _card)
    {
        this.fireballPrefab = fireballPrefab;

        this.damage = _card.EffectValue;
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

            fireball.GetComponent<Fireball>().Launch(damage, direction);
        }

        Debug.Log($"Fireball Barrage activated: {fireballCount} fireballs launched.");
    }

    public void Disable()
    {
        Debug.Log("Fireball Barrage does not require a disable phase.");
    }

      public void ApplySynergy(float synergyBonus)
    {
        throw new System.NotImplementedException();
    }

}
