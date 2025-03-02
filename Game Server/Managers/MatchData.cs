using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Managers
{
    public class MatchData
    {
        private Dictionary<string, SearchData> _playersData;

        public Dictionary<string, SearchData> PlayersData { get => _playersData; }

        private int _matchId;
        public int MatchId { get => _matchId;}

        public MatchData(int matchId, List<SearchData> passedData)
        {
            _matchId = matchId;
            _playersData = new Dictionary<string, SearchData>();
            foreach (SearchData s in passedData) 
            {
                s.IsReady = false;
                _playersData.Add(s.UserId,s);
            }
        }

        public void ChangePlayerReady(string userId,bool isReady)
        {
            if(_playersData.ContainsKey(userId))
                _playersData[userId].IsReady = isReady;
        }

        public bool IsAllReady()
        {
            foreach(string userId in _playersData.Keys)
            {
                if (_playersData[userId].IsReady == false)
                    return false;
            }
            return true;
        }
    }
}
