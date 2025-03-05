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
        //להוסיף כאן סרוויס שישלח לשחקן השני את הצעד כדי שיעדכן את הלוח שלו 

        public object Handle(User user, Dictionary<string, object> details)
        {
            string MoveData = null;
            //{"Service","SendMove"} && {"MoveData",int}
            Dictionary<string, object> response = new Dictionary<string, object>();
            if (details.ContainsKey("Service") && details.ContainsKey("MoveData"))
            {
                GameRoom room = _roomManager.GetRoom(user.MatchId);
                response.Add("Service", "PassTurn");
                if (room != null)
                {
                   
                    MoveData = details["MoveData"].ToString();
                    response = room.ReceivedMove(user, MoveData);
                    response.Add("RoomId", room.RoomId);
                    return response;    
                }
                
                    
                   
            }
            return response;
        }
    }
}
