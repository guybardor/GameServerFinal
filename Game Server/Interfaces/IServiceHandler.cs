using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Interfaces
{
    public interface IServiceHandler
    {
        string ServiceName { get; }
        object Handle(User user, Dictionary<string, object> details);
    }
}
