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
    
    public bool IsGameFinished { get; set; } = false;
    public int WinnerId { get; set; }
    
    public bool IsPaused { get; set; } = false;
    public int? PlayerOnDisconnectId { get; set; }
    
    public int P1DisconnectCount { get; set; } = 0;
    public TimeSpan P1RemainingPauseTime { get; set; }
    
    public int P2DisconnectCount { get; set; } = 0;
    public TimeSpan P2RemainingPauseTime { get; set; } = TimeSpan.FromMinutes(5);
    
    private System.Timers.Timer? _pauseTimer;
    private DateTime _pauseStartTime;
    
    public delegate void TimeoutHandler(string gameId, int loserId);
    public event TimeoutHandler? OnPauseTimeout;
    
    public int P1TotalShots { get; set; } = 0;
    public int P1Hits { get; set; } = 0;

    public int P2TotalShots { get; set; } = 0;
    public int P2Hits { get; set; } = 0;

    public bool MakeShot(int shooterId, int x, int y)
    {
        if (!IsGameStarted || IsGameFinished || IsPaused || shooterId != CurrentTurnPlayerId)
        {
            return false;
        }

        if (shooterId == Player1Id)
        {
            P1TotalShots++;
        }
        else
        {
            P2TotalShots++;
        }

        bool isHit = (shooterId == Player1Id) ? Board2[x, y] : Board1[x, y];
        
        if (isHit)
        {
            if (shooterId == Player1Id)
            {
                P1Hits++;
                Board2[x, y] = false;
            }
            else
            {
                P2Hits++;
                Board1[x, y] = false;
            }
            
            CheckForWin(shooterId);
        }
        
        else
        {
            CurrentTurnPlayerId = (shooterId == Player1Id) ? Player2Id : Player1Id;
        }
        
        return isHit;
    }
    
    public bool StartPause(int disconnectedPlayerId)
    {
        if (IsGameFinished || !IsGameStarted || IsPaused) return false;

        if (disconnectedPlayerId == Player1Id && P1DisconnectCount >= 5)
        {
            return false;
        }

        if (disconnectedPlayerId == Player2Id && P2DisconnectCount >= 5)
        {
            return false;
        }

        IsPaused = true;
        PlayerOnDisconnectId = disconnectedPlayerId;
        _pauseStartTime = DateTime.UtcNow;


        if (disconnectedPlayerId == Player1Id)
        {
            P1DisconnectCount++;
        }
        else
        {
            P2DisconnectCount++;
        }
        
        TimeSpan totalAvailable = (disconnectedPlayerId == Player1Id) ? P1RemainingPauseTime : P2RemainingPauseTime;
        TimeSpan maxForThisTurn = TimeSpan.FromMinutes(3);
        
        TimeSpan timeForThisPause = totalAvailable < maxForThisTurn ? totalAvailable : maxForThisTurn;

        if (timeForThisPause.TotalMilliseconds <= 0)
        {
            return false; 
        }
        
        _pauseTimer = new System.Timers.Timer(timeForThisPause.TotalMilliseconds);
        _pauseTimer.Elapsed += (sender, e) => HandleTimeout(disconnectedPlayerId);
        _pauseTimer.AutoReset = false;
        _pauseTimer.Start();

        return true;
    }
    
    public void StopPause(int reconnectedPlayerId)
    {
        if (!IsPaused || PlayerOnDisconnectId != reconnectedPlayerId)
        {
            return;
        }
        
        if (_pauseTimer != null)
        {
            _pauseTimer.Stop();
            _pauseTimer.Dispose();
        }
        
        TimeSpan timeSpent = DateTime.UtcNow - _pauseStartTime;
        
        if (reconnectedPlayerId == Player1Id)
        {
            P1RemainingPauseTime -= timeSpent;
            if (P1RemainingPauseTime < TimeSpan.Zero)
            {
                P1RemainingPauseTime = TimeSpan.Zero;
            }
        }
        else
        {
            P2RemainingPauseTime -= timeSpent;
            if (P2RemainingPauseTime < TimeSpan.Zero)
            {
                P2RemainingPauseTime = TimeSpan.Zero;
            }
        }
        
        IsPaused = false;
        PlayerOnDisconnectId = null;
    }
    
    private void HandleTimeout(int loserId)
    {
        IsGameFinished = true;
        WinnerId = (loserId == Player1Id) ? Player2Id : Player1Id;
        
        if (_pauseTimer != null) _pauseTimer.Dispose();
        
        OnPauseTimeout?.Invoke(GameId, loserId);
    }
    
    private void CheckForWin(int shooterId)
    {
        bool[,] opponentBoard = (shooterId == Player1Id) ? Board2 : Board1;

        foreach (var cell in opponentBoard)
        {
            if (cell)
            {
                return; 
            }
        }
        
        IsGameFinished = true;
        WinnerId = shooterId;
    }
}