using TicTacToeGameServer.Interfaces;

namespace TicTacToeGameServer.Services.Redis
{
    public class RatingRedisService : IRatingRedisService
    {
        private readonly IRedisBaseService _redisBaseService;

        public RatingRedisService(IRedisBaseService redisBaseService)
        {
            _redisBaseService = redisBaseService;
        }

        public string GetPlayerRating(string userId)
        {
            return _redisBaseService.GetString($"{userId}#Rating");
        }

        public void SetPlayerRating(string userId, string rating)
        {
            _redisBaseService.SetString($"{userId}#Rating", rating);
        }
    }
}
