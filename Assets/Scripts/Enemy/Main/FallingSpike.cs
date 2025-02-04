using UnityEngine;

public class FallingSpike : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private GameObject shadowPrefab;
    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private Collider2D col;
    private bool hasLanded = false;
    private GameObject shadowInstance; // Store reference to the shadow

    private void Start() => CreateShadow();

    private void Update()
    {
        if (!hasLanded)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }
    }

    private void CreateShadow()
    {
        Vector2 groundPosition = new Vector2(transform.position.x, Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0f)).y + 0.5f);
        shadowInstance = Instantiate(shadowPrefab, groundPosition, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out CharacterManager player))
        {
            player.TakeDamage(damage);
            col.enabled = false;
            Destroy(gameObject);
        }
        else if (shadowInstance != null && other.gameObject == shadowInstance)
        {
            Instantiate(impactPrefab, shadowInstance.transform.position, Quaternion.identity);
            Destroy(gameObject); 
            Destroy(shadowInstance);
        }
    }

    private void OnBecameInvisible() => Destroy(gameObject);
}
