using System.Collections.Generic;
using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Services.ClientRequests
{
    public class SendMoveRequest : IServiceHandler
    {
        private readonly RoomsManager _roomManager;
        public SendMoveRequest(RoomsManager roomManager)
        {
            _roomManager = roomManager;
        }

        public string ServiceName => "SendMove";

        public object Handle(User user, Dictionary<string, object> details)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            if (details.ContainsKey("Index"))
            {
                GameRoom room = _roomManager.GetRoom(user.MatchId);
                if (room != null)
                    response = room.ReceivedMove(user, details["Index"].ToString());
            }
            return response;
        }
    }
}
