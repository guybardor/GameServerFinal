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
            string MoveData = null;
           
            Dictionary<string, object> response = new Dictionary<string, object>();
            if (details.ContainsKey("Service") && details.ContainsKey("MoveData"))
            {

                if (_roomManager.ActiveRooms[user.MatchId] != null) 
                {
                    response.Add("Service", "PassTurn");
                    MoveData = details["MoveData"].ToString();
                    response = _roomManager.ActiveRooms[user.MatchId].ReceivedMove(user, MoveData);
                    return response;
                }
               
               
               
                
                    
                   
            }
            return response;
        }
    }
}

