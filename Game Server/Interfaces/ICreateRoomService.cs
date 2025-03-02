using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Interfaces
{
    public interface ICreateRoomService
    {
        Dictionary<string, object> Create(User user,Dictionary<string, object> Matchdeatails);
    }
}
