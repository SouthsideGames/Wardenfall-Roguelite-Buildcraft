using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardShopPool", menuName = "criptable Objects/Card Shop Pool", order = 10)]
public class CardShopPoolSO : ScriptableObject
{
    [Header("Available Cards")]
    [SerializeField] private List<CardSO> cards;
    public List<CardSO> Cards => cards;
}
