using WebSocketSharp.Server;

namespace TicTacToeGameServer.Extensions
{
    public static class WebSocketSessionExtensions
    {
        public static async Task SendAsync(this IWebSocketSession session, string message)
        {
            if (session?.Context?.WebSocket != null && session.Context.WebSocket.IsAlive)
            {
                await Task.Run(() => session.Context.WebSocket.Send(message));
            }
        }

        public static async Task SendAsync(this IWebSocketSession session, byte[] data)
        {
            if (session?.Context?.WebSocket != null && session.Context.WebSocket.IsAlive)
            {
                await Task.Run(() => session.Context.WebSocket.Send(data));
            }
        }

        public static async Task SendAsync(this IWebSocketSession session, object obj)
        {
            if (session?.Context?.WebSocket != null && session.Context.WebSocket.IsAlive)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                await session.SendAsync(json);
            }
        }
    }
}
