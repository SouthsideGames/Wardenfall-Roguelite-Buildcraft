using UnityEngine;

public class IceHazard : EnvironmentalHazard
{
    [SerializeField] private float slowdownFactor = 0.5f;

    protected override void Start()
    {
        base.Start();
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