using UnityEngine;

public class VoidRift : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int damage = 30;
    [SerializeField] private float delay = 2f;

    private void Start()
    {
        Invoke(nameof(Explode), delay);
    }

    private void Explode()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hitEnemies)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<CharacterManager>().TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}
