using UnityEngine;

public class SecondLifeEffect : ICardEffect
{
    private GameObject explosionPrefab;
    private CardSO cardSO;

    public SecondLifeEffect(GameObject explosionPrefab, CardSO cardSO)
    {
        this.explosionPrefab = explosionPrefab;
        this.cardSO = cardSO;
    }

    public void Activate(float duration)
    {
        CharacterHealth playerHealth = CharacterManager.Instance.health;

        playerHealth.Heal(Mathf.RoundToInt(playerHealth.maxHealth / 2));

        GameObject explosion = GameObject.Instantiate(explosionPrefab, CharacterManager.Instance.transform.position, Quaternion.identity);
        Explosion explosionScript = explosion.GetComponent<Explosion>();
        explosionScript.SetDamage(Mathf.RoundToInt(playerHealth.maxHealth));
    }

    public void Disable()
    {
        
    }


}
