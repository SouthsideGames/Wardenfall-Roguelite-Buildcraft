using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    private CharacterManager characterManager;
    private List<GameObject> activeEffects = new();

    private void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
        if (characterManager == null)
        {
            Debug.LogError("CharacterManager component not found on this GameObject.");
            enabled = false; // Disable this script if CharacterManager is not found
        }
    }

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

        // Start cooldown after activeTime delay
        StartCoroutine(DelayedCooldown(card, slotUI));
    }

    private IEnumerator DelayedCooldown(CardSO card, InGameCardSlotUI slotUI)
    {
        yield return new WaitForSeconds(card.cooldown);
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
