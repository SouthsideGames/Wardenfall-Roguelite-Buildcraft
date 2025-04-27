using UnityEngine;


public class SpikeHazard : EnvironmentalHazard
{
    protected override void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            ApplyHazardEffect(other);
        else if (other.CompareTag("Enemy"))
            ApplyHazardEffect(other);
    }
}
