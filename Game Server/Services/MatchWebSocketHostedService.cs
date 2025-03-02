using TicTacToeGameServer.Interfaces.WebSocketInterfaces;

namespace TicTacToeGameServer.Services
{
    public class MatchWebSocketHostedService : IHostedService
    {
        private readonly IMatchWebSocketService _matchWebSocketService;

        public MatchWebSocketHostedService(IMatchWebSocketService matchWebSocketService)
        {
            _matchWebSocketService = matchWebSocketService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _matchWebSocketService.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _matchWebSocketService.Stop();
            return Task.CompletedTask;
        }
    }
}
