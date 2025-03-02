using Newtonsoft.Json;
using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;
using WebSocketSharp.Server;

namespace TicTacToeGameServer.Services
{
    public class ConnectionRequest : IConnectionRequest
    {
        private readonly SessionManager _sessionManager;
        private readonly SearchingManager _searchingManager;
        private readonly IdToUserIdManager _idToUserIdManager;
        private readonly IRatingRedisService _ratingRedisService;

        public ConnectionRequest(SessionManager sessionManager,
            SearchingManager searchingManager,
            IdToUserIdManager idToUserIdManager,
            IRatingRedisService ratingRedisService)
        {
            _sessionManager = sessionManager;
            _searchingManager = searchingManager;
            _ratingRedisService = ratingRedisService;
            _idToUserIdManager = idToUserIdManager;
        }

        public Task<bool> OpenAsync(IWebSocketSession session, string id, string details)
        {
            Console.WriteLine(id + " " + details);
            try
            {
                Dictionary<string,string> userData = JsonConvert.DeserializeObject<Dictionary<string,string>>(details);
                if (userData != null && userData.ContainsKey("UserId"))
                {
                    string userId = userData["UserId"];
                    User newUser = new User(userId,session);
                    newUser.SetMatchingState();

                    _sessionManager.AddUser(newUser);
                    _idToUserIdManager.AddMapping(session.ID,userId);

                    string rating = _ratingRedisService.GetPlayerRating(userId);
                    int.TryParse(rating, out int playerRatingValue);

                    if (playerRatingValue > 0)
                    {
                        _searchingManager.AddToSearch(userId, playerRatingValue);
                        return Task.FromResult(true);
                    }
                }
            }
            catch (Exception ex) { }
            return Task.FromResult(false);
        }
    }
}
