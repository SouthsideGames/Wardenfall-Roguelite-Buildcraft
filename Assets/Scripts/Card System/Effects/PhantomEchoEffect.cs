using UnityEngine;

public class PhantomEchoEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject decoyPrefab;
    [SerializeField] private float aggroRadius = 8f;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || decoyPrefab == null)
        {
            Debug.LogWarning("PhantomEchoEffect: Target or prefab missing.");
            return;
        }

        GameObject decoy = Instantiate(decoyPrefab, target.transform.position, Quaternion.identity);
        Decoy decoyScript = decoy.GetComponent<Decoy>();
        if (decoyScript != null)
        {
            decoyScript.Initialize(card.activeTime, aggroRadius);
        }

        Destroy(gameObject, card.activeTime + 0.5f);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}