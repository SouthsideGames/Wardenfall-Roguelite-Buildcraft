using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardFilterButtonUI : MonoBehaviour
{
    [SerializeField] private CardEffectType effectType;

    public void AssignEffectType() => DeckManager.Instance.FilterCards(effectType);
}
