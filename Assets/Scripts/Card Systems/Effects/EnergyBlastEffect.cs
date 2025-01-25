using UnityEngine;

public class EnergyBlastEffect : ICardEffect
{
    private GameObject energyOrbPrefab;
    private CardSO cardSO;

    public EnergyBlastEffect(GameObject _energyOrbPrefab, CardSO _cardSO)
    {
        energyOrbPrefab = _energyOrbPrefab;
        cardSO = _cardSO;
    }

    public void Activate(float duration)
    {
        Vector2 spawnPosition = CharacterManager.Instance.transform.position;
        Vector2 direction = CharacterManager.Instance.GetAimDirection(); 

        GameObject orb = Object.Instantiate(energyOrbPrefab, spawnPosition, Quaternion.identity);
        orb.GetComponent<EnergyOrb>().Launch(direction, cardSO);
    }

    public void Disable()
    {
    }



}
