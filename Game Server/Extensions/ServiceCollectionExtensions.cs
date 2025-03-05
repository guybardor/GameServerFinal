using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TicTacToeGameServer.Configurations;
using TicTacToeGameServer.Handlers;
using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Interfaces.WebSocketInterfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Services;
using TicTacToeGameServer.Services.AddServices;
using TicTacToeGameServer.Services.ClientRequests;
using TicTacToeGameServer.Services.HostedServices;
using TicTacToeGameServer.Services.Redis;
using TicTacToeGameServer.Services.Requests;

namespace TicTacToeGameServer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddService(this IServiceCollection serviceCollection)
        {
            var serviceHandlers = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IServiceHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

            foreach (var type in serviceHandlers)
            {
                serviceCollection.AddSingleton(typeof(IServiceHandler), type);
            }

            serviceCollection.AddSingleton<IDictionary<string, IServiceHandler>>(sp =>
            {
                var handlers = sp.GetServices<IServiceHandler>();
                return handlers.ToDictionary(h => h.ServiceName, h => h);
            });

            return serviceCollection
                .AddConfiguration()
                .AddSingleton<IMatchWebSocketService, MatchWebSocketService>()
                .AddSingleton<ICloseRequest, CloseRequest>()
                .AddSingleton<IConnectionRequest, ConnectionRequest>()
                .AddSingleton<IRedisBaseService, RedisBaseService>()
                .AddSingleton<IRatingRedisService, RatingRedisService>()
                .AddSingleton<IProcessRequest, ProcessRequest>()
                .AddSingleton<IMessageService, MessageService>()
                .AddSingleton<ICreateRoomService, CreateRoomService>()
                .AddSingleton<IMatchIdRedisService, MatchIdRedisService>()
                .AddSingleton<IRandomizerService, RandomizerService>()
                .AddSingleton<IDateTimeService, DateTimeService>()
                .AddTransient<ConnectionHandler>()
                .AddSingleton<SessionManager>()
                .AddSingleton<SearchingManager>()
                .AddSingleton<MatchingManager>()
                .AddSingleton<IdToUserIdManager>()
                .AddSingleton<RoomsManager>()
                /*.AddHostedService<MatchMakingHostedService>()*/
                /*.AddHostedService<GameLoopHostedService>()*/
                .AddHostedService<MatchWebSocketHostedService>();

        }

        private static IServiceCollection AddConfiguration(this IServiceCollection serviceCollection)
        {
            serviceCollection
           .AddOptions<WebSocketSharpConfiguration>()
           .BindConfiguration("WebSocketSharp")
           .ValidateDataAnnotations()
           .ValidateOnStart();

            return serviceCollection;
        }
    }
}
