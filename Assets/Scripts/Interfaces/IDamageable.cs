using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage, bool isCritical = false);
    void Die();
    bool IsAlive { get; }
    int CurrentHealth { get; }
    int MaxHealth { get; }
}
