using UnityEngine;

public interface ICardEffect
{
    void Activate(CharacterManager target, CardSO card);

    void Deactivate();

    void Tick(float deltaTime);
}
