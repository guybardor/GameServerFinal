using LobbyServer.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace LobbyServer.Services
{
    public class PlayersRedisService : IPlayersRedisService
    {
        private readonly IRedisBaseService _redisBaseService;

        public PlayersRedisService(IRedisBaseService redisBaseService)
        {
            _redisBaseService = redisBaseService;
        }

        public Dictionary<string, string> GetPlayer(string email)
        {
            return _redisBaseService.GetDictionary($"{email}#Players");
        }

        public void SetPlayer(string email, Dictionary<string, string> playerData)
        {
            _redisBaseService.SetDictionary($"{email}#Players", playerData);
        }
    }
}
