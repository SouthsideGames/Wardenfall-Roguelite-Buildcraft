using UnityEngine;

public class EnergyBlastEffect : ICardEffect
{
    private GameObject energyOrbPrefab;
    private CardSO cardSO;

    public EnergyBlastEffect(GameObject energyOrbPrefab, CardSO cardSO)
    {
        this.energyOrbPrefab = energyOrbPrefab;
        this.cardSO = cardSO;
    }

    public void Activate(float duration)
    {
        Vector2 spawnPosition = CharacterManager.Instance.transform.position;
        Vector2 direction = CharacterManager.Instance.GetAimDirection(); 

        GameObject orb = Object.Instantiate(energyOrbPrefab, spawnPosition, Quaternion.identity);
        orb.GetComponent<EnergyOrb>().Launch(direction, cardSO);

        Debug.Log("Energy Blast activated! Orb launched.");
    }

    public void Disable()
    {
        Debug.Log("Energy Blast has no disable phase.");
    }

      public void ApplySynergy(float synergyBonus)
    {
        throw new System.NotImplementedException();
    }

}
