using UnityEngine;

public class BlackHole : MonoBehaviour
{
     [SerializeField] private float pullForce = 5f;
    [SerializeField] private float duration = 5f;
    private Transform player;

    private void Start()
    {
        player = FindFirstObjectByType<CharacterManager>().transform;
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        Vector3 direction = (transform.position - player.position).normalized;
        player.position += direction * pullForce * Time.deltaTime;
    }
}
