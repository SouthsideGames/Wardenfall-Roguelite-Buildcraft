using UnityEngine;

public class WeaponTrail : MonoBehaviour
{
    private TrailRenderer trail;

    void Start()
    {
        trail = GetComponentInChildren<TrailRenderer>();
        trail.enabled = false;
    }

    public void EnableTrail() => trail.enabled = true;

    public void DisableTrail() => trail.enabled = false;
}
