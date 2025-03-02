using System.Threading;
using System.Threading.Tasks;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Services.HostedServices
{
   /* public class GameLoopHostedService : BackgroundService
    {
        private const int _millisecondsDelay = 500;
        private readonly RoomsManager _roomManager;
        public GameLoopHostedService(RoomsManager roomManager)
        {
            _roomManager = roomManager;
        }

        *//*protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                UpdateGameLoop();
                await Task.Delay(_millisecondsDelay,stoppingToken);
            }
        }*/

        /*private void UpdateGameLoop()
        {
            Dictionary<string, GameRoom> activeRooms = _roomManager.ActiveRooms;
            if (activeRooms != null)
            {
                foreach (string s in activeRooms.Keys)
                {
                    activeRooms[s]?.GameLoop();
                }
            }
        }*//*
    }*/
}
