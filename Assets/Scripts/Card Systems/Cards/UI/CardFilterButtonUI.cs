using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardFilterButtonUI : MonoBehaviour
{
    [SerializeField] private CardType effectType;

    public void AssignEffectType() => CardManager.Instance.FilterCards(effectType);
}
