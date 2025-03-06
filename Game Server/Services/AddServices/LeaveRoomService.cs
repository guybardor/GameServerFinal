using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Services.AddServices
{
    
    public class LeaveRoomService: IServiceHandler
    {
        private readonly RoomsManager _roomManager;
        public string ServiceName => "LeaveRoom";
        public LeaveRoomService(RoomsManager roomManager)
        {
            _roomManager = roomManager;
        }

        public object Handle(User user, Dictionary<string, object> details)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            if (details.ContainsKey("RoomId") && details["RoomId"] != null )
            {
                string RoomId = details["RoomId"].ToString();
                GameRoom gameRoom = null;
                gameRoom = _roomManager.GetRoom(RoomId);

                if (gameRoom != null) 
                {
                    gameRoom.LeaveRoom(user);
                }

            }
            return response;
        }
    }
}
