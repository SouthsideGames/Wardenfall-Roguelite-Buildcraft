using System.Collections.Generic;
using UnityEngine;

public class BladeStormEffect : ICardEffect
{
    private GameObject bladePrefab;
    private CardSO cardSO;
    private List<GameObject> spawnedBlades = new List<GameObject>();   

    public BladeStormEffect(GameObject _bladePrefab, CardSO _card)
    {
        bladePrefab = _bladePrefab;
        cardSO = _card;    
    }

    public void Activate(float duration)
    {
        Vector2 spawnPosition = (Vector2)CharacterManager.Instance.transform.position + Random.insideUnitCircle * 4;
        GameObject blade = Object.Instantiate(bladePrefab, spawnPosition, Quaternion.identity);
        blade.GetComponent<Blade>().Configure(cardSO);

         spawnedBlades.Add(blade);
    }

    public void Disable()
    {
        foreach (var blade in spawnedBlades)
        {
            if (blade != null)
            {
                Object.Destroy(blade);
            }
        }

        spawnedBlades.Clear();
    }

}
