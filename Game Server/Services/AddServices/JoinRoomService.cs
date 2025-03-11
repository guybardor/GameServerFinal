using System.Text.RegularExpressions;
using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;
using TicTacToeGameServer.Services.Redis;

namespace TicTacToeGameServer.Services.AddServices
{
    public class JoinRoomService : IServiceHandler
    {
        private readonly RoomsManager _roomsManager;
        private readonly MatchingManager _matchingManager;
        private readonly IRatingRedisService _ratingRedisService;
        private bool  IsJoinToMatchingManger = false;
        private List<SearchData> ListSearchData;

        public string ServiceName => "JoinRoom"; // ✅ Implements interface

        public JoinRoomService(RoomsManager roomsManager, MatchingManager matchingManager, IRatingRedisService ratingRedisService)
        {
            _roomsManager = roomsManager;
            Console.WriteLine($"[JoinRoomService] Using RoomsManager Instance HashCode: {_roomsManager.GetHashCode()}");
            _matchingManager = matchingManager;
            _ratingRedisService = ratingRedisService;
            ListSearchData = new List<SearchData>();    
        }

        public object Handle(User user, Dictionary<string, object> details)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("Response", "JoinRoom");

            if (!details.ContainsKey("RoomId") || user==null)
            {
                result.Add("Success", false);
                result.Add("Message", "Missing RoomId or UserId");
                return result;
            }

            string roomId = details["RoomId"].ToString();
            string userId = user.UserId;





            bool Success = _roomsManager.AddUserToRoom(roomId, user);


            int rating = int.Parse(_ratingRedisService.GetPlayerRating(userId));

            SearchData searchData = new SearchData(user.UserId, rating);
            if (searchData != null) 
            {
                ListSearchData.Add(searchData);
                MatchData matchData = new MatchData(int.Parse(roomId), ListSearchData);
                IsJoinToMatchingManger = _matchingManager.AddToMatchingData(roomId, matchData);



            }
           
          


            if (!Success || !IsJoinToMatchingManger)
            {
                result.Add("IsSuccess", Success);
                result.Add("Message", "Failed to join room");
                return result;
            }


            result.Add("IsSuccess", true);
            result.Add("RoomId", roomId);
            result.Add("Message", "User joined room successfully");
            return result;
        }
    }
}
