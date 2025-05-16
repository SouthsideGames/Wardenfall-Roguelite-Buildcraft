using UnityEngine;

public class BulletModifierLoader : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    
    private void Start()
    {
        var cards = CharacterManager.Instance.cards;

        if (cards.HasCard("recoil_boost") && !GetComponent<RecoilModifier>())
            gameObject.AddComponent<RecoilModifier>();

        if (cards.HasCard("death_mark") && !GetComponent<DeathMarkModifier>())
            gameObject.AddComponent<DeathMarkModifier>();

        if (cards.HasCard("pierce_shot") && !GetComponent<PierceModifier>())
            gameObject.AddComponent<PierceModifier>();

        if (cards.HasCard("split_fire") && !GetComponent<SplitFireModifier>())
        {
            var split = gameObject.AddComponent<SplitFireModifier>();
            split.GetType().GetField("bulletPrefab").SetValue(split, bulletPrefab); // inject prefab
        }// Add more conditions here for other bullet modifiers
        // Example:
        // if (cards.HasCard("piercing_rounds")) AddComponent<PiercingModifier>();
    }
}