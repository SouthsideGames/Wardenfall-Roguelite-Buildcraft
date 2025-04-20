using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBehavior
{
    void Initialize(EnemyDataSO data);
    void UpdateBehavior();
    void OnHit();
    void ApplyEffect(StatusEffect effect);
}
