using UnityEngine;


public class FireballBarrageEffect : ICardEffect
{
    private GameObject fireballPrefab;
    private int fireballCount = 5;
    private float spawnRadius = 10;
    private float damage;

    public FireballBarrageEffect(GameObject _fireballPrefab, CardSO _card)
    {
        fireballPrefab = _fireballPrefab;

        damage = _card.EffectValue;
    }

    public void Activate(float duration)
    {
        for (int i = 0; i < fireballCount; i++)
        {
            Vector2 spawnPosition = (Vector2)CharacterManager.Instance.transform.position + Random.insideUnitCircle * spawnRadius;

            Vector2 targetPosition = spawnPosition + Random.insideUnitCircle.normalized;

            GameObject fireball = Object.Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);
            Vector2 direction = (targetPosition - spawnPosition).normalized;

            fireball.GetComponent<Fireball>().Launch((int)damage, direction);
        }
    }

    public void Disable()
    {
    
    }


}
