using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Interfaces
{
    public interface IMessageService
    {
        public Task<object> HandleMessageAsync(User curUser,string data);
    }
}
