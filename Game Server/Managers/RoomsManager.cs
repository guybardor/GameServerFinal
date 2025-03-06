using System.Diagnostics;
using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Managers
{
    public class RoomsManager
    {
        private Dictionary<string, GameRoom> _activeRooms;
        public Dictionary<string, GameRoom> ActiveRooms { get => _activeRooms; }

        public RoomsManager() 
        {
            Console.WriteLine("Roommanger Constructor");
           
            _activeRooms = new Dictionary<string, GameRoom>();
        }

        public bool AddRoom(string matchId,GameRoom gameRoom)
        {
            Debug.WriteLine("we Are Add a New Room");
            if (_activeRooms == null)
                _activeRooms = new Dictionary<string, GameRoom>();
                  

            if(_activeRooms.ContainsKey(matchId))
                _activeRooms[matchId] = gameRoom;
            else _activeRooms.Add(matchId,gameRoom);
            return true;
        }

        public void RemoveRoom(string matchId) 
        {
             if(_activeRooms != null && _activeRooms.ContainsKey(matchId))
                _activeRooms.Remove(matchId);
        }

        public GameRoom GetRoom(string matchId)
        {
            if (_activeRooms != null && _activeRooms.ContainsKey(matchId))
                return _activeRooms[matchId];
            return null;
        }

        public bool IsRoomExist(string matchId)
        {
            if(GetRoom(matchId) != null)
                return true;    
            return false;
        }

        public List<GameRoom> GetAllActiveRooms()
        {
            List<GameRoom> roomsList = new List<GameRoom>();

            foreach (var room in _activeRooms.Values)
            {
                if (room != null || room.Users.Count < room.MaxUsersCount) // ✅ Ensure the room exists
                {
                    
                    roomsList.Add(room);
                }
            }

            return roomsList;
        }

        public bool AddUserToRoom(string roomId, User user)
        {
            bool Success = false;
            if (ActiveRooms == null || ActiveRooms[roomId].Users.Count + 1 > ActiveRooms[roomId].MaxUsersCount)
            {
                return Success;
            }
            else {
                Success = ActiveRooms[roomId].AddUser(user.UserId.ToString(),user);
                if (Success == true) 
                {
                    user.MatchId = ActiveRooms[roomId].RoomId;
                }
            }

            return Success; 
        }

        public bool SubscibeUserToRoom(string roomId, User user)
        {
            bool Success = false;
            if (ActiveRooms == null )
            {
                return Success;
            }
            else
            {
                Success = ActiveRooms[roomId].SubscribeToRoom(user.UserId.ToString(), user);
            }

            return Success;
        }

        public Dictionary<string, object> StopGame(string matchId, User user, string winner)
        {

            Dictionary<string,object> response = new Dictionary<string,object>();       
            response = ActiveRooms[matchId].StopGame(user, winner);
            if (response != null)
            {
                return response;
            }
            else {
                return null;
            }
             




        }

        public Dictionary<string, object> LeaveRoom(string roomId)
        {
                    
            Dictionary<string, object> response = new Dictionary<string, object>()
            {
                { "Response", "RemoveRoom" },
                { "IsSuccess", false },
                { "RoomId", roomId },
                { "Owner", "" },
                { "MaxUsers", 0 },
                { "Name", "" }
            };

            
            if (_activeRooms == null || !_activeRooms.ContainsKey(roomId))
            {
                return response;
                // Room not found => "IsSuccess" remains false
            }

            
            GameRoom room = _activeRooms[roomId];
            response["Owner"] = room.Owner;
            response["MaxUsers"] = room.MaxUsersCount;
            response["Name"] = room.Name;

           
            var currentUsers = room.Users.Values.ToList();
            foreach (var user in currentUsers)
            {
                room.LeaveRoom(user);
            }

            // 4) Remove the room from the dictionary
            _activeRooms.Remove(roomId);
            response["IsSuccess"] = true;

            return response;
        }

    }
}
