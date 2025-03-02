using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Services.AddServices
{
    public class JoinRoomService : IServiceHandler
    {
        private readonly RoomsManager _roomsManager;

        public string ServiceName => "JoinRoom"; // ✅ Implements interface

        public JoinRoomService(RoomsManager roomsManager)
        {
            _roomsManager = roomsManager;
            Console.WriteLine($"[JoinRoomService] Using RoomsManager Instance HashCode: {_roomsManager.GetHashCode()}");
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
           
            if (!Success)
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
