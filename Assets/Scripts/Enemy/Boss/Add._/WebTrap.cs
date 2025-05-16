using System.Collections;
using UnityEngine;

public class WebTrap : MonoBehaviour
{
    [SerializeField] private float effectDuration = 2f; // Duration of immobilization

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController player = other.GetComponent<CharacterController>();
            if (player != null)
            {
                player.DisableMovement(effectDuration); // Freeze player
            }
        }
    }
}
