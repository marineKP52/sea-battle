namespace Server.Logic;

public class GameSession
{
    public string GameId { get; set; } = Guid.NewGuid().ToString();
    public int Player1Id { get; set; }
    public int Player2Id { get; set; }
    
    public bool[,] Board1 { get; set; } = new bool[10, 10];
    public bool[,] Board2 { get; set; } = new bool[10, 10];

    public int CurrentTurnPlayerId { get; set; }
    public bool IsGameStarted { get; set; } = false;

    public bool MakeShot(int shooterId, int x, int y)
    {
        if (shooterId != CurrentTurnPlayerId) return false;

        bool isHit = (shooterId == Player1Id) ? Board2[x, y] : Board1[x, y];

        
        if (!isHit)
        {
            CurrentTurnPlayerId = (shooterId == Player1Id) ? Player2Id : Player1Id;
        }
        
        return isHit;
    }
}