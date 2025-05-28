using UnityEngine;

public class ArcTurretEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject arcTurretPrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || arcTurretPrefab == null)
        {
            Debug.LogWarning("ArcTurretEffect: Target or prefab is missing.");
            return;
        }

        Vector3 spawnPos = target.transform.position;
        GameObject turret = Instantiate(arcTurretPrefab, spawnPos, Quaternion.identity);

        ArcTurret turretLogic = turret.GetComponent<ArcTurret>();
        if (turretLogic != null)
        {
            turretLogic.Initialize(card.activeTime);
        }

        Destroy(gameObject, card.activeTime + 0.5f);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
