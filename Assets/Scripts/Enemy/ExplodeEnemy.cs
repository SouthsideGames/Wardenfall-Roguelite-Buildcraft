using UnityEngine;

public class ExplodeEnemy : Enemy
{
    [Header("EXPLODER SPECIFICS:")]
    [SerializeField] private float explosionRadius = 3f; 
    [SerializeField] private int explosionDamage = 10; 
    [SerializeField] private ParticleSystem explosionEffect; 
    private bool isExploding = false; 

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

        anim.SetTrigger("Explode");
     
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
