using UnityEngine;

public class OversizeModifier : MonoBehaviour, IBulletModifier
{
    [SerializeField] private float sizeMultiplier = 1.5f;

    public void Apply(BulletBase bullet, Enemy target)
    {
        if (bullet == null) return;
        bullet.transform.localScale *= sizeMultiplier;
    }
}
