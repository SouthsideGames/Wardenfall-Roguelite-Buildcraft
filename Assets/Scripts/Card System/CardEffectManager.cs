using System;
using System.Collections;
using System.Collections.Generic;
using SouthsideGames.DailyMissions;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    public static Action OnCardActivated;
    private List<GameObject> activeEffects = new();
    private Dictionary<string, int> cardUsageCounts = new Dictionary<string, int>();

    public string GetMostUsedCard()
    {
        string mostUsedCardId = "";
        int maxUsage = 0;

        foreach (var kvp in cardUsageCounts)
        {
            if (kvp.Value > maxUsage)
            {
                maxUsage = kvp.Value;
                mostUsedCardId = kvp.Key;
            }
        }

        return mostUsedCardId;
    }

    public void ActivateCard(CardSO card, InGameCardSlotUI slotUI)
    {
        if (card == null)
        {
            Debug.LogWarning("Attempted to activate null card");
            return;
        }

        WaveManager.Instance?.AdjustViewerScore(0.05f);
        MissionManager.Increment(MissionType.cardsUsed, 1);

        // Track card usage
        if (!cardUsageCounts.ContainsKey(card.cardID))
            cardUsageCounts[card.cardID] = 0;
        cardUsageCounts[card.cardID]++;

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

            effect.Activate(CharacterManager.Instance, card);
            activeEffects.Add(effectObj);

            OnCardActivated?.Invoke();

            HandleCooldown(card, slotUI);
        }
        catch (Exception e)
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

    private void OnDestroy() => ClearEffects();
}
