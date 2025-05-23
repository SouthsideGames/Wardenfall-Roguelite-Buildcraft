
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameManagerTests
{
    [Test]
    public void GameState_WhenStartingNewGame_ShouldBeMenu()
    {
        // Arrange
        var gameObject = new GameObject();
        var gameManager = gameObject.AddComponent<GameManager>();

        // Act & Assert
        Assert.AreEqual(GameState.Menu, gameManager.gameState);
    }

    [Test]
    public void StartGame_ShouldSetGameStateToGame()
    {
        // Arrange
        var gameObject = new GameObject();
        var gameManager = gameObject.AddComponent<GameManager>();

        // Act
        gameManager.StartGame();

        // Assert
        Assert.AreEqual(GameState.Game, gameManager.gameState);
    }

    [Test]
    public void StartShop_ShouldSetGameStateToShop()
    {
        // Arrange
        var gameObject = new GameObject();
        var gameManager = gameObject.AddComponent<GameManager>();

        // Act
        gameManager.StartShop();

        // Assert
        Assert.AreEqual(GameState.Shop, gameManager.gameState);
    }
}
