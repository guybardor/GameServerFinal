namespace TicTacToeGameServer.Interfaces
{
    public interface IRatingRedisService
    {
        public string GetPlayerRating(string userId);

        public void SetPlayerRating(string userId, string rating);
    }
}
