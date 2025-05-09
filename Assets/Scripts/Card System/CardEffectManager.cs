
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;
    private List<GameObject> activeEffects = new();

    private void Awake()
    {
        if (characterManager == null)
        {
            characterManager = FindObjectOfType<CharacterManager>();
        }
    }

    public void ActivateCard(CardSO card, InGameCardSlotUI slotUI)
    {
        if (card == null)
        {
            Debug.LogWarning("Attempted to activate null card");
            return;
        }

        if (card.effectPrefab == null)
        {
            Debug.LogWarning($"Card {card.cardName} has no effect prefab");
            return;
        }

        try
        {
            GameObject effectObj = Instantiate(card.effectPrefab);
            ICardEffect effect = effectObj.GetComponent<ICardEffect>();

            if (effect == null)
            {
                Debug.LogError($"Effect prefab for card {card.cardName} does not implement ICardEffect");
                Destroy(effectObj);
                return;
            }

            effect.Activate(characterManager, card);
            activeEffects.Add(effectObj);

            HandleCooldown(card, slotUI);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error activating card {card.cardName}: {e.Message}");
        }
    }

    private void HandleCooldown(CardSO card, InGameCardSlotUI slotUI)
    {
        if (slotUI == null) return;

        if (card.cooldownStartsOnUse)
        {
            slotUI.TriggerCooldown(card.cooldown);
        }
        else
        {
            StartCoroutine(DelayedCooldown(card, slotUI));
        }
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
            {
                var cardEffect = effect.GetComponent<ICardEffect>();
                if (cardEffect != null)
                {
                    cardEffect.Deactivate();
                }
                Destroy(effect);
            }
        }
        activeEffects.Clear();
    }

    private void OnDestroy()
    {
        ClearEffects();
    }
}
