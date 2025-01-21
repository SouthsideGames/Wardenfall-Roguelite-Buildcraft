using System.Collections;
using UnityEngine;

public class MoltenTrailEffect : ICardEffect
{
    private GameObject moltenTrailPrefab;
    private CardSO cardSO;

    public MoltenTrailEffect(GameObject _moltenTrailPrefab, CardSO _cardSO)  
    {
        cardSO = _cardSO;   
        moltenTrailPrefab = _moltenTrailPrefab;
    }

    public void Activate(float activeTime)
    {
        GameObject moltenTrail = Object.Instantiate(moltenTrailPrefab, CharacterManager.Instance.transform.position, Quaternion.identity);
        MoltenTrail trailScript = moltenTrail.GetComponent<MoltenTrail>();

        trailScript.Initialize(cardSO);

    }

    public void Disable()
    {
        Debug.Log("Molten Trail does not require a disable phase.");
    }
}