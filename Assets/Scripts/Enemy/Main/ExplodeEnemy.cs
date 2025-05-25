using System.Collections;
using UnityEngine;

public class ExplodeEnemy : Enemy
{
    [Header("EXPLODER SPECIFICS:")]
    [SerializeField] private float explosionRadius = 3f; 
    [SerializeField] private int explosionDamage = 10; 
    [SerializeField] private ParticleSystem explosionEffect; 
    private bool isExploding = false; 
    
    private float panicTimer;
    private Vector3 originalScale;
    private float panicIntensity = 0f;


    protected override void Start()
    {
        base.Start();
        originalScale = transform.localScale;
        StartCoroutine(PanicBehavior());
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || !CanAttack() || isExploding)
            return;

        if (IsPlayerTooClose())
        {
            Explode();
        }
        else
        {

            movement.FollowCurrentTarget();
        }
    }


    private bool IsPlayerTooClose()
    {
        // Check if the player is within the trigger distance for explosion
        return Vector2.Distance(transform.position, character.transform.position) <= playerDetectionRadius;
    }

    private void Explode()
    {
       
        if (isExploding) return;
        isExploding = true;
     
        if (explosionEffect != null)
        {
            ParticleSystem effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            effect.Play();
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
  
            if (hit.TryGetComponent<Enemy>(out Enemy enemy) && enemy != this)
            {
                enemy.TakeDamage(explosionDamage, false);
            }


            if (hit.TryGetComponent<CharacterManager>(out CharacterManager player))
            {
                player.TakeDamage(explosionDamage);
            }
        }

    }

    private IEnumerator PanicBehavior()
    {
        while (true)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer < 5f)
            {
                panicTimer += Time.deltaTime;
                panicIntensity = Mathf.Min(panicTimer / 2f, 1f); // Builds up over 2 seconds

                transform.localScale = originalScale * (1f + Mathf.Sin(Time.time * (10f + panicIntensity * 5f)) * (0.2f * panicIntensity));
                transform.position += (Vector3)Random.insideUnitCircle * (0.1f * panicIntensity);
            }
            else
            {
                panicTimer = Mathf.Max(0f, panicTimer - Time.deltaTime * 2f); // Calms down twice as fast
                panicIntensity = panicTimer / 2f;
                transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 5f);
            }

            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
