using UnityEngine;

public class HealingSurgeEffect : MonoBehaviour, ICardEffect
{
    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || card == null)
        {
            return;
        }

        CharacterHealth health = target.GetComponent<CharacterHealth>();
        if (health != null)
        {
            float healAmount = health.maxHealth * card.effectValue;
            health.Heal((int)healAmount);
        }

        Destroy(gameObject);
    }

    public void Deactivate() { }

    public void Tick(float deltaTime) { }
}
