using NUnit.Framework;
using FluentAssertions;
using Server.Logic;

namespace Server.Tests;

[TestFixture]
public class GameManagerTests
{
    private GameManager _gameManager;

    [SetUp]
    public void Setup()
    {
        _gameManager = new GameManager();
    }

    [Test]
    public void JoinQueue_WhenFirstPlayerJoins_ShouldReturnNull()
    {
        var game = _gameManager.JoinQueue(playerId: 1);
        
        game.Should().BeNull();
    }

    [Test]
    public void JoinQueue_WhenSecondPlayerJoins_ShouldCreateAndReturnActiveGame()
    {
        _gameManager.JoinQueue(playerId: 1);
        
        var game = _gameManager.JoinQueue(playerId: 2);
        
        game.Should().NotBeNull();
        game!.Player1Id.Should().Be(1);
        game.Player2Id.Should().Be(2);
        game.IsGameStarted.Should().BeFalse();
    }

    [Test]
    public void JoinQueue_WhenPlayerTriesToJoinTwice_ShouldBlockDuplicateAndReturnNull()
    {
        _gameManager.JoinQueue(playerId: 1);
        
        var secondAttemptGame = _gameManager.JoinQueue(playerId: 1);
        
        secondAttemptGame.Should().BeNull();
        
        var matchGame = _gameManager.JoinQueue(playerId: 2);
        matchGame.Should().NotBeNull();
        
        var emptyCheckGame = _gameManager.JoinQueue(playerId: 3);
        emptyCheckGame.Should().BeNull();
    }

    [Test]
    public void LeaveQueue_WhenPlayerCancelsSearch_ShouldRemoveHimFromQueue()
    {
        _gameManager.JoinQueue(playerId: 1);
        _gameManager.LeaveQueue(playerId: 1);
        
        var game = _gameManager.JoinQueue(playerId: 2);
        
        game.Should().BeNull();
    }
}