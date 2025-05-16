using UnityEngine;

public class PierceModifier : MonoBehaviour, IBulletModifier
{
    [SerializeField] private int maxPierces = 2;
    private int pierces = 0;

    public void Apply(BulletBase bullet, Enemy target)
    {
        if (target == null) return;

        pierces++;

        if (pierces >= maxPierces)
        {
            bullet.Release();
        }
        else
        {
            // Prevent bullet from being destroyed/released by default logic
            bullet.CancelDestroyOnHit();
        }
    }
} 
