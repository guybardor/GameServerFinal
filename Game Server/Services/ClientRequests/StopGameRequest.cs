using System.Collections.Generic;
using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Services.ClientRequests
{
    public class StopGameRequest : IServiceHandler
    {
        private readonly RoomsManager _roomManager;
        public StopGameRequest(RoomsManager roomManager)
        {
            _roomManager = roomManager;
        }

        public string ServiceName => "StopGame";

        public object Handle(User user, Dictionary<string, object> details)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            if (details.ContainsKey("Winner"))
            {
                GameRoom room = _roomManager.GetRoom(user.MatchId);
                if (room != null)
                    response = room.StopGame(user, details["Winner"].ToString());
            }
            return response;
        }
    }
}
