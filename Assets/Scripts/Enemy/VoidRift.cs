using UnityEngine;

public class VoidRift : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int damage = 30;
    [SerializeField] private float delay = 2f;
    [SerializeField] private GameObject explosionPrefab;

    private CharacterManager player;
    private float summonTimer;

    private void Start()
    {
        summonTimer = delay / 2;
        Invoke(nameof(Explode), delay);
        
    }

    private void Update()
    {

        summonTimer -= Time.deltaTime;
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

        if(summonTimer <= 0)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
