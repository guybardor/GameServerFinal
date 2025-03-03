using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;
using System.Collections.Generic;

namespace TicTacToeGameServer.Services.AddServices
{
    public class GetRoomsInRangeService : IServiceHandler
    {
        private readonly RoomsManager _roomsManager;

        public string ServiceName => "GetRoomsInRange"; // ✅ Properly implements interface

        public GetRoomsInRangeService(RoomsManager roomsManager)
        {
            _roomsManager = roomsManager;
            Console.WriteLine($"[GetRoomInRange!#@!#!#!] Using RoomsManager Instance HashCode: {_roomsManager.GetHashCode()}");
        }

        public object Handle(User user, Dictionary<string, object> details)
        {
            Dictionary<string, object> Result = new Dictionary<string, object>();
            List<GameRoom> activeRooms = new List<GameRoom> { };
            // Add Response Key
            Result.Add("Response", "GetRoomsInRange");
            if (details.ContainsKey("MinUserCount") && details.ContainsKey("MaxUserCount") && int.Parse(details["MinUserCount"].ToString()) > 0 && int.Parse(details["MinUserCount"].ToString()) < 2
                 )   
            {
                activeRooms = _roomsManager.GetAllActiveRooms();

                if (activeRooms == null || activeRooms.Count == 0) 
                {
                    return Result;
                }
                
                Result.Add("Service", "UserJoinRoom");
                   
                
                
                Result.Add("Rooms", activeRooms);
                

            }
            return Result;
            
       


        }
    }
}
