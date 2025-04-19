using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [Header("DAMAGE SETTINGS")]
    [SerializeField] private int damageAmount = 10; // Set damage value in the Inspector
    [SerializeField] private float lifetime;

    private float currentTime;

    private void Start() {
        currentTime = lifetime;
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;  

        if(currentTime < 0)
          Destroy(gameObject);  
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterManager.Instance.health.TakeDamage(damageAmount);
        }
    }
}
