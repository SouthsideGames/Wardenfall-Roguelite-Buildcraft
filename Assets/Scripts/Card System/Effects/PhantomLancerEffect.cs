using UnityEngine;

public class PhantomLancerEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject phantomLancerPrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || phantomLancerPrefab == null)
        {
            Debug.LogWarning("PhantomLancerEffect: Target or prefab not assigned.");
            return;
        }

        GameObject lancer = Instantiate(phantomLancerPrefab, target.transform.position, Quaternion.identity);
        PhantomLancerAI logic = lancer.GetComponent<PhantomLancerAI>();

        if (logic != null)
        {
            logic.Initialize(card.activeTime);
        }
    }

    public void Deactivate()
    {
        // No persistent state to deactivate
    }

    public void Tick(float deltaTime)
    {
        // Not needed for this effect
    }
}
