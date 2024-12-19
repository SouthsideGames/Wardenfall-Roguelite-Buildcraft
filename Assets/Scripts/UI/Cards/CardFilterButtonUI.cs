using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardFilterButtonUI : MonoBehaviour
{
    [SerializeField] private CardEffectType effectType;

    public void AssignEffectType() => LoadoutManager.Instance.FilterCards(effectType);
}
