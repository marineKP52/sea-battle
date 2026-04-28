namespace Server.Logic;
public class GameManager
{
    private readonly List<GameSession> _activeGames = new();

    public GameSession CreateGame(int p1Id, int p2Id)
    {
        var game = new GameSession { Player1Id = p1Id, Player2Id = p2Id, CurrentTurnPlayerId = p1Id };
        _activeGames.Add(game);
        return game;
    }

    public GameSession? GetGameByPlayer(int playerId)
    {
        return _activeGames.FirstOrDefault(g => g.Player1Id == playerId || g.Player2Id == playerId);
    }
}