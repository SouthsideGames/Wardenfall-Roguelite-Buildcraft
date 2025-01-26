using UnityEngine;

public class PlasmaBeamEffect : ICardEffect
{
    private GameObject plasmaBeamPrefab;
    private CardSO cardSO;

    public PlasmaBeamEffect(GameObject _plasmaBeamPrefab, CardSO _cardSO)
    {
        plasmaBeamPrefab = _plasmaBeamPrefab;
        cardSO = _cardSO;
    }

    public void Activate(float duration)
    {
        Vector2 playerPosition = CharacterManager.Instance.transform.position;
        GameObject plasmaBeam = Object.Instantiate(plasmaBeamPrefab, playerPosition, Quaternion.identity);

        PlasmaBeam beamScript = plasmaBeam.GetComponent<PlasmaBeam>();
        beamScript.Configure((int)cardSO.EffectValue, cardSO.ActiveTime);

    }

    public void Disable() {}


}
