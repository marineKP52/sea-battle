using Server.Logic;
using Server.Data;
using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs;

public class GameHub : Hub
{
    private readonly GameManager _gameManager;
    private readonly ApplicationDbContext _context;

    public GameHub(GameManager gameManager, ApplicationDbContext context)
    {
        _gameManager = gameManager;
        _context = context;
    }
    
    public override async Task OnConnectedAsync()
    {
        if (int.TryParse(Context.UserIdentifier, out int playerId))
        {
            var game = _gameManager.GetGameByPlayer(playerId);
            if (game != null && game.IsPaused && game.PlayerOnDisconnectId == playerId)
            {
                game.StopPause(playerId);
                
                await Groups.AddToGroupAsync(Context.ConnectionId, game.GameId);

                int opponentId = (game.Player1Id == playerId) ? game.Player2Id : game.Player1Id;
                
                await Clients.Group(game.GameId).SendAsync("PlayerReconnected", playerId);
                await Clients.Group(game.GameId).SendAsync("GameResumed", game.CurrentTurnPlayerId);
                
                await Clients.Caller.SendAsync("RestoreGameState", new {
                    gameId = game.GameId,
                    currentTurn = game.CurrentTurnPlayerId,
                    yourId = playerId
                });
            }
        }
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (int.TryParse(Context.UserIdentifier, out int playerId))
        {
            var game = _gameManager.GetGameByPlayer(playerId);
            
            if (game != null && game.IsGameStarted && !game.IsGameFinished)
            {
                game.OnPauseTimeout += HandleTechnicalTimeout;
                
                bool pauseStarted = game.StartPause(playerId);

                if (pauseStarted)
                {
                    int opponentId = (game.Player1Id == playerId) ? game.Player2Id : game.Player1Id;
                    
                    TimeSpan totalAvailable = (playerId == game.Player1Id) ? game.P1RemainingPauseTime : game.P2RemainingPauseTime;
                    double allowedSeconds = Math.Min(180, totalAvailable.TotalSeconds);
                    
                    await Clients.User(opponentId.ToString()).SendAsync("PlayerDisconnected", new {
                        msg = "Суперник від'єднався. Очікування...",
                        timeoutSeconds = allowedSeconds
                    });
                }
                else
                {
                    await ProcessGameOverInDatabase(game, winnerId: (game.Player1Id == playerId) ? game.Player2Id : game.Player1Id);
                }
            }
        }
        await base.OnDisconnectedAsync(exception);
    }
    
    private async void HandleTechnicalTimeout(string gameId, int loserId)
    {
        var game = _gameManager.GetGameByPlayer(loserId);
        if (game != null)
        {
            int winnerId = (game.Player1Id == loserId) ? game.Player2Id : game.Player1Id;
            await ProcessGameOverInDatabase(game, winnerId);
        }
    }
    
    private async Task ProcessGameOverInDatabase(GameSession game, int winnerId)
    {
        int loserId = (winnerId == game.Player1Id) ? game.Player2Id : game.Player1Id;
        
        var winner = await _context.Users.FindAsync(winnerId);
        var loser = await _context.Users.FindAsync(loserId);

        if (winner != null && loser != null)
        {
            winner.Rating += 30;
            winner.GamesPlayed += 1;

            loser.Rating -= 15;
            if (loser.Rating < 0) loser.Rating = 0;
            loser.GamesPlayed += 1;
            
            if (game.P1TotalShots >= 10)
            {
                float p1Accuracy = (float)game.P1Hits / game.P1TotalShots;
                if (p1Accuracy > 0.9f && !game.IsPaused) 
                {
                    var player1 = (game.Player1Id == winnerId) ? winner : loser;
                    player1.IsSuspicious = true;
                    // player1.Stats.LastSuspiciousMatchId = game.GameId; 
                }
            }
            
            if (game.P2TotalShots >= 10)
            {
                float p2Accuracy = (float)game.P2Hits / game.P2TotalShots;
                if (p2Accuracy > 0.9f && !game.IsPaused)
                {
                    var player2 = (game.Player2Id == winnerId) ? winner : loser;
                    player2.IsSuspicious = true;
                }
            }

            await _context.SaveChangesAsync();
        }
        
        await Clients.Group(game.GameId).SendAsync("GameOver", new {
            winnerId = winnerId,
            p1Shots = game.P1TotalShots,
            p1Hits = game.P1Hits,
            p2Shots = game.P2TotalShots,
            p2Hits = game.P2Hits
        });
        
        _gameManager.RemoveGame(game.GameId);
    }
    
    public async Task FindGame()
    {
        if (!int.TryParse(Context.UserIdentifier, out int playerId))
        {
            return;
        }
        
        var activeGame = _gameManager.GetGameByPlayer(playerId);
        if (activeGame != null)
        {
            await Clients.Caller.SendAsync("AlreadyInGame", activeGame.GameId);
            return;
        }

        var game = _gameManager.JoinQueue(playerId);

        if (game != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, game.GameId);
        
            await Clients.User(game.Player1Id.ToString()).SendAsync("GameFound", game.GameId);
            await Clients.User(game.Player2Id.ToString()).SendAsync("GameFound", game.GameId);
        
            await Clients.Group(game.GameId).SendAsync("PrepareBoard");
        }
        else
        {
            await Clients.Caller.SendAsync("WaitingForOpponent");
        }
    }
    
    public async Task CancelSearch()
    {
        if (int.TryParse(Context.UserIdentifier, out int playerId))
        {
            _gameManager.LeaveQueue(playerId);
            await Clients.Caller.SendAsync("SearchCancelled");
        }
    }
    
    public async Task UploadBoard(bool[,] clientBoard)
    {
        if (!int.TryParse(Context.UserIdentifier, out int playerId))
        {
            return;
        }

        var game = _gameManager.GetGameByPlayer(playerId);
        if (game == null)
        {
            return;
        }
        
        await Groups.AddToGroupAsync(Context.ConnectionId, game.GameId);
        
        if (playerId == game.Player1Id)
        {
            game.Board1 = clientBoard;
        }
        else if (playerId == game.Player2Id)
        {
            game.Board2 = clientBoard;
        }

        
        bool isBoard1Ready = HasShips(game.Board1);
        bool isBoard2Ready = HasShips(game.Board2);

        if (isBoard1Ready && isBoard2Ready)
        {
            game.IsGameStarted = true;
            await Clients.Group(game.GameId).SendAsync("GameStarted", game.CurrentTurnPlayerId);
        }
        else
        {
            await Clients.Caller.SendAsync("WaitingForOpponentBoard");
        }
    }
    
    private bool HasShips(bool[,] board)
    {
        foreach (var cell in board)
        {
            if (cell) return true;
        }
        return false;
    }

    public async Task PlayerShoot(int x, int y)
    {
        if (!int.TryParse(Context.UserIdentifier, out int playerId))
        {
            return;
        }

        var game = _gameManager.GetGameByPlayer(playerId);
        
        if (game != null && game.IsGameStarted && !game.IsGameFinished)
        {
            bool hit = game.MakeShot(playerId, x, y);
            int opponentId = (game.Player1Id == playerId) ? game.Player2Id : game.Player1Id;

            await Clients.User(playerId.ToString()).SendAsync("ShotResult", x, y, hit, true); 
            await Clients.User(opponentId.ToString()).SendAsync("ShotResult", x, y, hit, false); 
    
            if (game.IsGameFinished)
            {
                int winnerId = game.WinnerId;
                int loserId = (winnerId == game.Player1Id) ? game.Player2Id : game.Player1Id;
                
                var winner = await _context.Users.FindAsync(winnerId);
                var loser = await _context.Users.FindAsync(loserId);

                if (winner != null && loser != null)
                {
                    winner.Rating += 30;
                    winner.GamesPlayed += 1;
                    
                    loser.Rating -= 15;
                    if (loser.Rating < 0)
                    {
                        loser.Rating = 0;
                    }
                    
                    loser.GamesPlayed += 1;
                    
                    await _context.SaveChangesAsync();
                }
                
                await Clients.Group(game.GameId).SendAsync("GameOver", winnerId);
            }
            
            else
            {
                await Clients.Group(game.GameId).SendAsync("TurnChanged", game.CurrentTurnPlayerId);
            }
        }
    }
}