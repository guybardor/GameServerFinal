using WebSocketSharp.Server;

namespace TicTacToeGameServer.Interfaces
{
    public interface IProcessRequest
    {
        Task ProcessMessageAsync(IWebSocketSession session, string data);
    }
}
