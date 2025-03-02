namespace TicTacToeGameServer.Interfaces
{
    public interface IMatchIdRedisService
    {
        string GetMatchId();
        void SetMatchId(string matchId);    
    }
}
