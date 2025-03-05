/*using Microsoft.AspNetCore.Identity;
using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;

public class PassTurnRequest : IServiceHandler
{
    private readonly RoomsManager _roomManager;

    public PassTurnRequest(RoomsManager roomManager)
    {
        _roomManager = roomManager;
    }

    public string ServiceName => "PassTurn";

    public object Handle(User user, Dictionary<string, object> details)
    {
        // We can prepare a default response to return
        Dictionary<string, object> response = new Dictionary<string, object>();

        if (_roomManager.ActiveRooms[user.MatchId] != null)
        {
            response = _roomManager.ActiveRooms[user.MatchId].ChangeTurn();
            if ((response.ContainsKey("Service") && response["Service"].ToString() != null) && (response.ContainsKey("CP") && response["CP"].ToString() != null) && (response.ContainsKey("MC") && response["MC"].ToString() != null))
            {
                return response;

            }
            else
            {
                response.Add("Service", "PassTurn");


            }


        }
        return response;
    }    
}
*/