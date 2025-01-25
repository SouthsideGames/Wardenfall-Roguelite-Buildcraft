using UnityEngine;

public class GravityCollapseEffect : ICardEffect
{
    private GameObject gravityFieldPrefab;
    private float damage;
    private float duration;
    private float pullRadius;

    public GravityCollapseEffect(GameObject _gravityFieldPrefab, CardSO _cardSO, float _pullRadius)
    {
        gravityFieldPrefab = _gravityFieldPrefab;
        damage = _cardSO.EffectValue;
        duration = _cardSO.ActiveTime;
        pullRadius = _pullRadius;
    }

    public void Activate(float activeDuration)
    {
        Vector2 centerPosition = CharacterManager.Instance.transform.position;

        GameObject gravityField = Object.Instantiate(gravityFieldPrefab, centerPosition, Quaternion.identity);
        GravityField gravityScript = gravityField.GetComponent<GravityField>();

        if (gravityScript != null)
            gravityScript.Configure((int)damage, duration, pullRadius);
    }

    public void Disable()
    {
    }
}
