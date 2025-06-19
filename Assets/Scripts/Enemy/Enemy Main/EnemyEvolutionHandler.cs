using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyEvolutionHandler : MonoBehaviour
{
    [SerializeField] private bool isEvolvable = false;
    private bool hasEvolved = false;
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public bool CanEvolve() => isEvolvable && !hasEvolved && enemy.IsAlive;

    public void Evolve()
    {
        if (!CanEvolve()) return;

        hasEvolved = true;

        enemy.maxHealth = Mathf.FloorToInt(enemy.maxHealth * 1.5f);
        enemy.health = enemy.maxHealth;
        enemy.contactDamage = Mathf.FloorToInt(enemy.contactDamage * 1.5f);
        enemy.transform.localScale *= 1.25f;

        enemy.modifierHandler?.ApplyTraits();
        Debug.Log($"{gameObject.name} has evolved!");
    }
}