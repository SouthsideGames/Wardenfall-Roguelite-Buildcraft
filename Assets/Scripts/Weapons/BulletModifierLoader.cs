using UnityEngine;

public class BulletModifierLoader : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    private void Start()
    {
        var cards = CharacterManager.Instance.cards;

        if (cards.HasCard("S-033") && !GetComponent<RecoilModifier>())
            gameObject.AddComponent<RecoilModifier>();

        if (cards.HasCard("S-031") && !GetComponent<DeathMarkModifier>())
            gameObject.AddComponent<DeathMarkModifier>();

        if (cards.HasCard("S-032") && !GetComponent<PierceModifier>())
            gameObject.AddComponent<PierceModifier>();

        if (cards.HasCard("S-034") && !GetComponent<SplitFireModifier>())
        {
            var split = gameObject.AddComponent<SplitFireModifier>();
            split.GetType().GetField("bulletPrefab").SetValue(split, bulletPrefab);
        }
        
        if (cards.HasCard("S-082") && !GetComponent<OversizeModifier>())
            gameObject.AddComponent<OversizeModifier>();
        
    }
}