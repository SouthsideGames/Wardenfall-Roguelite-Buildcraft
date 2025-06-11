using UnityEngine;

public class DashCooldownReducerEffect : MonoBehaviour, ICardEffect
{
    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null)
        {
            Debug.LogWarning("DashCooldownReducerEffect: Target is null.");
            return;
        }

        CharacterAbility ability = target.GetComponent<CharacterAbility>();
        if (ability != null)
        {
            float reductionFactor = 1f - card.effectValue; // e.g., 0.3f → 70% cooldown
            ability.ApplyDashCooldownModifier(reductionFactor);
        }

        Destroy(gameObject);
    }

    public void Deactivate() { }

    public void Tick(float deltaTime) { }
}
