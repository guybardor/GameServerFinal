using System.Collections.Generic;
using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;
using TicTacToeGameServer.Services.Redis;

namespace TicTacToeGameServer.Services.AddServices
{
    public class CreateTurnRoomService : IServiceHandler
    {

        private readonly ICreateRoomService _createRoomService;
        private readonly SessionManager _sessionmanager;



        public string ServiceName => "CreateTurnRoom";

        public CreateTurnRoomService(ICreateRoomService createRoomService)
        {

            _createRoomService = createRoomService;
            



        }

        public object Handle(User user, Dictionary<string, object> details)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            response.Add("Response", "CreateTurnRoom");
                       
            if (!details.ContainsKey("Owner") || !details.ContainsKey("Name") || !details.ContainsKey("MaxUsers") ||
                !details.ContainsKey("TurnTime") || !details.ContainsKey("TableProperties"))
            {
                response.Add("IsSuccess", false);
                response.Add("ErrorMessage", "Missing required parameters.");
                return response;
            }

            if (details["Owner"].ToString() == null || details["Name"].ToString() == null || int.Parse(details["MaxUsers"].ToString()) < 3
                && details["TableProperties"] == null
                )
            {

                response.Add("IsSuccess", false);
                response.Add("ErrorMessage", "Missing required parameters.");
                return response;
            }
            response = _createRoomService.Create(user, details);
            return response;
        }
    }
}
