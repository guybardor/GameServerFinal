using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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

            if (_roomsManager.ActiveRooms[user.MatchId].Users.Count == 2 && _roomsManager.ActiveRooms[user.MatchId] != null  )
            {

                //SEND ONLY TO THE FIRST USER
                result.Add("Service", "UserJoinRoom");
                Dictionary<string,object> RoomData = new Dictionary<string,object>();
                RoomData.Add("RoomId", _roomsManager.ActiveRooms[user.MatchId].RoomId);
                RoomData.Add("Name", _roomsManager.ActiveRooms[user.MatchId].Name);
                RoomData.Add("TurnTime", _roomsManager.ActiveRooms[user.MatchId].TurnTime);
                RoomData.Add("Owner", _roomsManager.ActiveRooms[user.MatchId].Owner);
                RoomData.Add("MaxUsersCount", _roomsManager.ActiveRooms[user.MatchId].MaxUsersCount);
                RoomData.Add("JoinedUsersCount", _roomsManager.ActiveRooms[user.MatchId].Users.Count);
                result.Add("RoomData", RoomData);
                User first_user = _roomsManager.ActiveRooms[user.MatchId].Users.Values.FirstOrDefault();
                result.Add("UserId", user.UserId);
                string stringRoomData = JsonConvert.SerializeObject(result);
                first_user.SendMessage(stringRoomData);
                //*_roomsManager.ActiveRooms[user.MatchId].BroadcastToRoom(user, stringRoomData); *//*








            }

            result.Add("IsSuccess", true);
            result.Add("RoomId", roomId);
            result.Add("Message", "User joined room successfully");
            return result;
        }
    }
    
}
