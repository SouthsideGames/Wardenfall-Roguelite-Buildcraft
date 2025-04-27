using UnityEngine;

public class IceHazard : EnvironmentalHazard
{
    [SerializeField] private float slowdownFactor = 0.5f;
    [SerializeField] private Color iceColor = new Color(0.8f, 0.8f, 1f, 0.3f);
    
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = iceColor;
        }
    }

    protected override void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.SetMovementMultiplier(slowdownFactor);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.SetMovementMultiplier(1f);
            }
        }
    }
}