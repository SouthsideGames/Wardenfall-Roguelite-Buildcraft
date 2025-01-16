using UnityEngine;

public class EnergyBlastEffect : ICardEffect
{
    private GameObject energyOrbPrefab;

    public EnergyBlastEffect(GameObject energyOrbPrefab)
    {
        this.energyOrbPrefab = energyOrbPrefab;
    }

    public void Activate(float duration)
    {
        Vector2 spawnPosition = CharacterManager.Instance.transform.position;
        Vector2 direction = CharacterManager.Instance.GetAimDirection(); 

        GameObject orb = Object.Instantiate(energyOrbPrefab, spawnPosition, Quaternion.identity);
        orb.GetComponent<EnergyOrb>().Launch(direction);

        Debug.Log("Energy Blast activated! Orb launched.");
    }

    public void Disable()
    {
        Debug.Log("Energy Blast has no disable phase.");
    }
}
