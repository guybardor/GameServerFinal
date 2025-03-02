using System.Threading.Tasks;
using TicTacToeGameServer.Extensions;
using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;
using WebSocketSharp.Server;

namespace TicTacToeGameServer.Services
{
    public class ProcessRequest : IProcessRequest
    {
        private readonly IdToUserIdManager _idToUserIdManager;
        private readonly SessionManager _sessionManager;
        private readonly IMessageService _messageService;
        public ProcessRequest(IdToUserIdManager idToUserIdManager,
            SessionManager sessionManager, IMessageService messageService) 
        {
            _idToUserIdManager = idToUserIdManager;
            _sessionManager = sessionManager;
            _messageService = messageService;
        }

        public async Task ProcessMessageAsync(IWebSocketSession session, string data)
        {
            string responseClient = string.Empty;

            string userId = _idToUserIdManager.GetUserId(session.ID);
            User user = _sessionManager.GetUser(userId);

            var serviceResponse = await _messageService.HandleMessageAsync(user, data);
            if (serviceResponse != null)
            {
                responseClient = serviceResponse.ToString();
                session.SendAsync(responseClient);
            }
        }
    }
}
