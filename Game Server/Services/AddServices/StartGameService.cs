using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Services.AddServices;

public class StartGameService : IServiceHandler
{
    private readonly RoomsManager _roomManager;
    public string ServiceName => "StartGame";

    public StartGameService(RoomsManager roomManager)
    {
        _roomManager = roomManager;
    }

    public object Handle(User user, Dictionary<string, object> details)
    {
        Dictionary<string, object> response = new Dictionary<string, object>();

        

      
        GameRoom room = _roomManager.GetRoom(user.MatchId);

       
        if (room == null)
        {
            response.Add("Error", "Room not found");
            return response;
        }
        room.StartGame();



        response.Add("Message", "Game started successfully");
   
        return response;
    }
}
