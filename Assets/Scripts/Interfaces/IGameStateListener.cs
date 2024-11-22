using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStateListener
{
    void GameStateChangedCallback(GameState _gameState);

}

public interface IGameModeListener
{
    void GameModeChangedCallback(GameMode _gameMode);

}
