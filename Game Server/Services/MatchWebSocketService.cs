using Microsoft.Extensions.Options;
using TicTacToeGameServer.Configurations;
using TicTacToeGameServer.Handlers;
using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Interfaces.WebSocketInterfaces;
using WebSocketSharp.Server;

namespace TicTacToeGameServer.Services
{
    public class MatchWebSocketService : IMatchWebSocketService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<WebSocketSharpConfiguration> _webSocketSharpConfiguration;
        
        private WebSocketServer? _webSocketServer;

        public MatchWebSocketService(IServiceProvider serviceProvider,
            IOptions<WebSocketSharpConfiguration> webSocketSharpConfiguration) 
        {
            _serviceProvider = serviceProvider;
            _webSocketSharpConfiguration = webSocketSharpConfiguration;
        }

        public void Start()
        {
            /*_webSocketServer = new WebSocketServer(_webSocketSharpConfiguration.Value.Port!.Value);*/
            _webSocketServer = new WebSocketServer($"ws://127.0.0.1:{_webSocketSharpConfiguration.Value.Port!.Value}");


            _webSocketServer.KeepClean = false; 
            _webSocketServer.WaitTime = TimeSpan.FromMinutes(60); 
            _webSocketServer.AddWebSocketService<ConnectionHandler>(_webSocketSharpConfiguration.Value.Path, service =>
            {
                service.Initialize(_serviceProvider,
                    _serviceProvider.GetRequiredService<ICloseRequest>(),
                    _serviceProvider.GetRequiredService<IProcessRequest>()
                    );
            });

            _webSocketServer.Start();
        }

        public void Stop()
        {
            _webSocketServer?.Stop();
        }
    }
}
