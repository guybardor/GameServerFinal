using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Services.AddServices
{
    public class GetLiveRoomInfoService : IServiceHandler
    {
        private readonly RoomsManager _roomsManager;

        public string ServiceName => "GetLiveRoomInfo";

        public GetLiveRoomInfoService(RoomsManager roomsManager)
        {
            _roomsManager = roomsManager;
        }

        public object Handle(User user, Dictionary<string, object> details)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("Response", "GetLiveRoomInfo");



            string roomId = details["RoomId"].ToString();
            GameRoom room = _roomsManager.GetRoom(roomId);
     

            Dictionary<string, object> roomProperties = room.TableProperties;

            List<string> userIds = _roomsManager.ActiveRooms[roomId]._playersOrder; 


            result.Add("RoomData", _roomsManager.ActiveRooms[roomId]);
            result.Add("RoomProperties", roomProperties);
            result.Add("Users", userIds);

            return result;
        }
    }
}
