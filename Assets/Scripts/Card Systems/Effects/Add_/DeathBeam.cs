using UnityEngine;

public class DeathBeam : MonoBehaviour
{
    private GameObject target;

    public void Configure(GameObject target)
    {
        this.target = target;
        if (target.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(enemy.maxHealth, false);
        }

        Destroy(gameObject);
    }
}
