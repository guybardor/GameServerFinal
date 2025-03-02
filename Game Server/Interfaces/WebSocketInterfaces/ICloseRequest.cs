namespace TicTacToeGameServer.Interfaces.WebSocketInterfaces
{
    public interface ICloseRequest
    {
        Task CloseAsync (string id);
    }
}
