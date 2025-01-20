using UnityEngine;

public class PlasmaBeamEffect : ICardEffect
{
    private GameObject plasmaBeamPrefab;
    private CardSO cardSO;

    public PlasmaBeamEffect(GameObject plasmaBeamPrefab, CardSO cardSO)
    {
        this.plasmaBeamPrefab = plasmaBeamPrefab;
        this.cardSO = cardSO;
    }

    public void Activate(float duration)
    {
        Vector2 playerPosition = CharacterManager.Instance.transform.position;
        GameObject plasmaBeam = Object.Instantiate(plasmaBeamPrefab, playerPosition, Quaternion.identity);

        PlasmaBeam beamScript = plasmaBeam.GetComponent<PlasmaBeam>();
        beamScript.Configure(cardSO.EffectValue, cardSO.ActiveTime);

        Debug.Log($"Plasma Beam activated for {cardSO.ActiveTime}s, dealing {cardSO.EffectValue} damage per tick.");
    }

    public void Disable() {}


}
