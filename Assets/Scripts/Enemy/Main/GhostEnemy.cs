using System.Collections;
using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
     [SerializeField] private float speed = 1.5f;
    [SerializeField] private float fuseTime = 4f;
    [SerializeField] private GameObject explosionEffect;
    private Transform player;
    private bool detonateOnEnemy = false;
    public SpriteRenderer _spriteRenderer;

    private void Start()
    {
        player = CharacterManager.Instance.transform;
        StartCoroutine(DelayedExplode());
    }

    public void InitializeFrom(SpriteRenderer enemySprite, int tierLevel)
    {

         switch (tierLevel)
        {
            case 1: // Haunting
                speed = 1.5f;
                fuseTime = 4f;
                detonateOnEnemy = false;
                break;
            case 2: // Tormented
                speed = 2f;
                fuseTime = 6f;
                detonateOnEnemy = false;
                break;
            case 3: // Vengeful
                speed = 2.5f;
                fuseTime = 6f;
                detonateOnEnemy = true;
                break;
        }
    }

    private void Update()
    {
        if (player == null) return;
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
        ChangeDirections();

    }

    protected virtual void ChangeDirections()
    {
        if (player != null)
        {
            _spriteRenderer.flipX = player.position.x > transform.position.x;
        }
    }

    private IEnumerator DelayedExplode()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       if (other.CompareTag("Player") || (detonateOnEnemy && other.CompareTag("Enemy")))
            Explode();
    }

    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
