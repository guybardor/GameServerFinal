/*using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Services.HostedServices
{
    public class MatchMakingHostedService : BackgroundService
    {
        private readonly SearchingManager _searchingManager;
        private readonly SessionManager _sessionManager;
        private readonly MatchingManager _matchingManager;
        private readonly int _ratingDifference = 50;
        private int _matchId = 1;
        private int _delayTime = 1000;

        public MatchMakingHostedService(SearchingManager searchingManager, 
            SessionManager sessionManager, MatchingManager matchingManager) 
        {
            _searchingManager = searchingManager;
            _sessionManager = sessionManager;
            _matchingManager = matchingManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine(DateTime.UtcNow);
                MatchingPlayers();
                await Task.Delay(_delayTime);
            }
        }

        private void MatchingPlayers()
        {
            List<SearchData> searchData = _searchingManager.GetSearchingList();
            if(searchData != null && searchData.Count > 1)
            {
                List<SearchData> sortedData = searchData.OrderBy(value => value.Rating).ToList();
                for(int i = 0; i < sortedData.Count - 1;i++)
                {
                    if (sortedData[i] != null && sortedData[i + 1] != null)
                    {
                        SearchData firstUser = sortedData[i];
                        SearchData secondUser = sortedData[i + 1];

                        if(RatingMatched(firstUser.Rating,secondUser.Rating))
                        {
                            List<User> users = new List<User>()
                            {
                                _sessionManager.GetUser(firstUser.UserId),
                                _sessionManager.GetUser(secondUser.UserId)
                            };

                            bool isValid = CheckValidity(users);
                            if(isValid)
                            {
                                Dictionary<string, object> data = new Dictionary<string, object>()
                                {
                                    {"Service","ReadyToPlay"},
                                    {"TempMatchId",_matchId}
                                };

                                List<SearchData> tempSearchers = new List<SearchData>()
                                {
                                    firstUser, secondUser
                                };

                                MatchData matchData = new MatchData(_matchId, tempSearchers);
                                _matchingManager.AddToMatchingData(_matchId.ToString(), matchData);

                                string broadcastMessage = JsonConvert.SerializeObject(data);
                                foreach (User u in users)
                                {
                                    u.SendMessage(broadcastMessage);
                                    u.CurUserState = User.UserState.PrePlay;
                                    u.MatchId = _matchId.ToString();
                                    _sessionManager.UpdateUser(u);
                                    _searchingManager.RemoveToSearch(u.UserId);
                                }

                                _matchId++;
                                i++;
                            }
                        }
                    }
                    
                }
            }
        }

        private bool RatingMatched(int rating1,int rating2)
        {
            int calc = Math.Abs(rating1 - rating2);
            return calc <= _ratingDifference;
        }

        private bool CheckValidity(List<User> matchedUsers)
        {
            foreach(User u in matchedUsers)
            {
                if(CheckIfUserIsValid(u) == false)
                    return false;
            }
            return true;
        }

        private bool CheckIfUserIsValid(User user)
        {
            return user != null && user.CurUserState == User.UserState.Matching && user.IsLive();
        }
    }
}
*/