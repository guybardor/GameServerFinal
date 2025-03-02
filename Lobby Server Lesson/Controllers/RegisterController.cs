using LobbyServer.Interfaces;
using LobbyServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LobbyServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class RegisterController : Controller
    {
        private int startRating = 500;

        private readonly IPlayersRedisService _playersRedisService;
        private readonly IRatingRedisService _ratingRedisService;

        public RegisterController(IPlayersRedisService playersRedisService, IRatingRedisService ratingRedisService)
        {
            _playersRedisService = playersRedisService;
            _ratingRedisService = ratingRedisService;   
        }

        [HttpPost("Register")]
        public Dictionary<string, object> Register([FromBody] Dictionary<string, object> data)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (data.ContainsKey("Email") && data.ContainsKey("Password"))
            {
                string? email = data["Email"].ToString();
                string? password = data["Password"].ToString();
                Dictionary<string, string> playerData = _playersRedisService.GetPlayer(email);
                if (playerData.Count == 0)
                {//Create user

                    string userId = Guid.NewGuid().ToString();
                    Dictionary<string, string> registerData = new Dictionary<string, string>()
                    {
                        {"Email",email},
                        {"Password",password},
                        {"UserId",userId},
                        {"CreatedTime",DateTime.UtcNow.ToString()}
                    };

                    _playersRedisService.SetPlayer(email, registerData);
                    _ratingRedisService.SetPlayerRating(userId, startRating.ToString());
                    result.Add("IsCreated", true);
                }
                else // user was created already
                {
                    result.Add("IsCreated", false);
                    result.Add("ErrorMessage", "User Already Exist");
                }
            }
            else result.Add("ErrorMessage", "Missing Params");

            result.Add("Response", "Register");
            return result;
        }
    }
}
