using LobbyServer.Interfaces;
using LobbyServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LobbyServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IPlayersRedisService _playersRedisService;
        private readonly IRatingRedisService _ratingRedisService;
        private readonly IConfiguration _configuration;

        public LoginController(IPlayersRedisService playersRedisService, IRatingRedisService ratingRedisService, IConfiguration configuration)
        {
            _playersRedisService = playersRedisService;
            _ratingRedisService = ratingRedisService;
            _configuration = configuration;
        }

        [HttpGet("Login/{email}&{password}")]
        public Dictionary<string, object> Login(string email, string password)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            Dictionary<string, string> playerData = _playersRedisService.GetPlayer(email);
            if (playerData.Count > 0 && playerData.ContainsKey("Password"))
            {
                string playerDataPassword = playerData["Password"];
                if (playerDataPassword == password)
                {
                    result.Add("IsLoggedIn", true);
                    result.Add("UserId", playerData["UserId"]);
                    result.Add("Rating", _ratingRedisService.GetPlayerRating(playerData["UserId"]));
                    string gameServerUrl = _configuration["GameServer:ConnectionString"].ToString();
                    result.Add("GameServerUrl", gameServerUrl ?? "Not Found");


                }
                else
                {
                    result.Add("IsLoggedIn", false);
                    result.Add("ErrorMessage", "Wrong Password");
                }
            }
            else
            {
                result.Add("IsLoggedIn", false);
                result.Add("ErrorMessage", "Player Doesnt Exist");
            }

            result.Add("Response", "Login");
            return result;
        }

       
    }
}
