using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStateListener
{
    void GameStateChangedCallback(GameState _gameState);

}

