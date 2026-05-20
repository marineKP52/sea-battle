using NUnit.Framework;
using FluentAssertions;
using Server.Logic;

namespace Server.Tests;

[TestFixture]
public class GameSessionTests
{
    private GameSession _game;
    
    [SetUp]
    public void Setup()
    {
        _game = new GameSession
        {
            Player1Id = 1,
            Player2Id = 2,
            CurrentTurnPlayerId = 1,
            IsGameStarted = true
        };
        
        _game.Board1 = new bool[10, 10];
        _game.Board2 = new bool[10, 10];
    }

    [Test]
    public void MakeShot_WhenHit_ShouldKeepCurrentTurnAndRegisterHit()
    {
        _game.Board2[5, 5] = true; 
        
        bool result = _game.MakeShot(shooterId: 1, x: 5, y: 5);
        
        result.Should().BeTrue();
        _game.CurrentTurnPlayerId.Should().Be(1); 
        _game.Board2[5, 5].Should().BeFalse(); 
        _game.P1Hits.Should().Be(1); 
    }

    [Test]
    public void MakeShot_WhenMiss_ShouldChangeTurnToOpponent()
    {
        bool result = _game.MakeShot(shooterId: 1, x: 0, y: 0);
        
        result.Should().BeFalse(); 
        _game.CurrentTurnPlayerId.Should().Be(2); 
        _game.P1TotalShots.Should().Be(1); 
        _game.P1Hits.Should().Be(0);
    }

    [Test]
    public void MakeShot_WhenNotPlayerTurn_ShouldIgnoreShot()
    {
        bool result = _game.MakeShot(shooterId: 2, x: 1, y: 1);
        
        result.Should().BeFalse();
        _game.P2TotalShots.Should().Be(0);
    }
}