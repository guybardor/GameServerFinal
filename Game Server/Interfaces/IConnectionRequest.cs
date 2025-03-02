using WebSocketSharp.Server;

namespace TicTacToeGameServer.Interfaces
{
    public interface IConnectionRequest
    {
        Task<bool> OpenAsync(IWebSocketSession session, string id, string details);
    }
}
