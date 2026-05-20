namespace Server.Logic;
public class GameManager
{
    private readonly List<GameSession> _activeGames = new();
    private readonly Queue<int> _waitingPlayers = new();

    public GameSession CreateGame(int p1Id, int p2Id)
    {
        var game = new GameSession { Player1Id = p1Id, Player2Id = p2Id, CurrentTurnPlayerId = p1Id };
        _activeGames.Add(game);
        return game;
    }

    public GameSession? GetGameByPlayer(int playerId)
    {
        return _activeGames.FirstOrDefault(g => (g.Player1Id == playerId || g.Player2Id == playerId) && !g.IsGameFinished);
    }
    
    public void RemoveGame(string gameId)
    {
        var game = _activeGames.FirstOrDefault(g => g.GameId == gameId);
        if (game != null)
        {
            _activeGames.Remove(game);
        }
    }
    
    public GameSession? JoinQueue(int playerId)
    {
        lock (_waitingPlayers)
        {
            if (GetGameByPlayer(playerId) != null)
            {
                return null;
            }

            if (_waitingPlayers.Contains(playerId))
            {
                return null;
            }
            
            if (_waitingPlayers.Count > 0)
            {
                int firstInQueueId = _waitingPlayers.Peek();
                
                if (firstInQueueId == playerId)
                {
                    return null; 
                }
                
                int opponentId = _waitingPlayers.Dequeue();
                return CreateGame(opponentId, playerId);
            }
            
            _waitingPlayers.Enqueue(playerId);
            return null;
        }
    }
    
    public void LeaveQueue(int playerId)
    {
        lock (_waitingPlayers)
        {
            if (_waitingPlayers.Contains(playerId))
            {
                var currentEntries = _waitingPlayers.Where(id => id != playerId).ToList();
                _waitingPlayers.Clear();
                foreach (var id in currentEntries)
                {
                    _waitingPlayers.Enqueue(id);
                }
            }
        }
    }
}