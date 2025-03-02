using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace LobbyServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class SearchingOpponentController : Controller
    {
        private readonly string connectionString = string.Empty;
        public SearchingOpponentController(IConfiguration configuration)
        {
            connectionString = configuration["GameServer:ConnectionString"].ToString();
        }

        [HttpGet("SearchingOpponent/{userid}")]
        public Dictionary<string, object> SearchingOpponent(string userId)
        {
            return new Dictionary<string, object>()
            {
                { "ConnectionUrl",connectionString},
                { "Response","SearchingOpponent"}
            };
        }
    }
}
