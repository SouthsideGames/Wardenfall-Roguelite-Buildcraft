using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;
    private List<GameObject> activeEffects = new();

    public void ActivateCard(CardSO card, InGameCardSlotUI slotUI)
    {
        if (card.effectPrefab == null)
        {
            Debug.LogWarning($"No effectPrefab assigned for card: {card.cardName}");
            return;
        }

        GameObject effectObj = Instantiate(card.effectPrefab);
        ICardEffect effect = effectObj.GetComponent<ICardEffect>();

        if (effect == null)
        {
            Debug.LogError($"Effect prefab for card {card.cardName} does not implement ICardEffect.");
            Destroy(effectObj);
            return;
        }

        effect.Activate(characterManager, card);
        activeEffects.Add(effectObj);

        if (card.cooldownStartsOnUse)
            slotUI.TriggerCooldown(card.cooldown);
        else
            StartCoroutine(DelayedCooldown(card, slotUI));
    }

    private IEnumerator DelayedCooldown(CardSO card, InGameCardSlotUI slotUI)
    {
        yield return new WaitForSeconds(card.activeTime);
        slotUI.TriggerCooldown(card.cooldown);
    }

    public void ClearEffects()
    {
        foreach (GameObject effect in activeEffects)
        {
            if (effect != null)
                Destroy(effect);
        }
        activeEffects.Clear();
    }
}
