using Server.Logic;
using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs;

public class GameHub : Hub
{
    private readonly GameManager _gameManager;

    public GameHub(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public async Task PlayerShoot(int x, int y)
    {
        if (!int.TryParse(Context.UserIdentifier, out int playerId))
        {
            return;
        }

        var game = _gameManager.GetGameByPlayer(playerId);

        if (game != null)
        {
            bool hit = game.MakeShot(playerId, x, y);
            
            int opponentId = (game.Player1Id == playerId) ? game.Player2Id : game.Player1Id;

            
            await Clients.User(playerId.ToString()).SendAsync("ShotResult", x, y, hit, true); 
            await Clients.User(opponentId.ToString()).SendAsync("ShotResult", x, y, hit, false); 
    
            await Clients.Group(game.GameId).SendAsync("TurnChanged", game.CurrentTurnPlayerId);
        }
    }
}