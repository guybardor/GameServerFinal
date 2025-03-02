using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;
using System;
using System.Collections.Generic;

namespace TicTacToeGameServer.Services.AddServices
{
    public class SubscribeRoomService : IServiceHandler
    {
        private readonly RoomsManager _roomsManager;

        public string ServiceName => "SubscribeRoom"; // ✅ Service Name Matches Client Request

        public SubscribeRoomService(RoomsManager roomsManager)
        {
            _roomsManager = roomsManager;
            Console.WriteLine($"[SubscribeRoomService] Using RoomsManager Instance HashCode: {_roomsManager.GetHashCode()}");
        }

        public object Handle(User user, Dictionary<string, object> details)
        {
            
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("Response", "SubscribeRoom");

            if (!details.ContainsKey("RoomId") || user == null)
            {
                result.Add("Success", false);
                result.Add("Message", "Missing RoomId or UserId");
                return result;
            }

            string roomId = details["RoomId"].ToString();
            string userId = user.UserId;





            bool Success = _roomsManager.SubscibeUserToRoom(roomId, user);

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
