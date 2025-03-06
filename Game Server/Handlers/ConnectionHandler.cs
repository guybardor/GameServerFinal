using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Interfaces.WebSocketInterfaces;
using TicTacToeGameServer.Services;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TicTacToeGameServer.Handlers
{
    public class ConnectionHandler : WebSocketBehavior
    {
        private IServiceProvider _serviceProvider;
        private ICloseRequest _closeRequest;
        private IProcessRequest _processRequest;

        public void Initialize(IServiceProvider serviceProvider, ICloseRequest closeRequest,
           IProcessRequest processRequest)
        {
            _serviceProvider = serviceProvider;
            _closeRequest = closeRequest;
            _processRequest = processRequest;
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            _processRequest.ProcessMessageAsync(Sessions[ID], e.Data);
        }

        protected override async void OnOpen()
        {
            if(Context.IsWebSocketRequest)
            {
                Console.WriteLine("On open MultiHandler " + ID);
                if (!Context.QueryString.AllKeys.Contains("data"))
                {
                    Console.WriteLine("Connection data is missing");
                    return;
                }

                var rawData = Context.QueryString["data"].ToString();
                if (rawData is null)
                {
                    Console.WriteLine("Connection data is empty");
                    return;
                }

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var connectionRequest = scope.ServiceProvider.GetRequiredService<IConnectionRequest>();
                    var openConnectionResult = await connectionRequest.OpenAsync(Sessions[ID], ID, rawData);
                    if (!openConnectionResult)
                    {
                        Sessions[ID].Context.WebSocket.Close(2020);
                    }
                }
                catch (Exception e) { }
            }
        }
        protected override void OnClose(CloseEventArgs closeEventArgs)
        {
            //להסיר את היוזר שיצא 
            _closeRequest.CloseAsync(ID);
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs errorEventArgs)
        {
            _closeRequest.CloseAsync(ID);
        }
    }
}
