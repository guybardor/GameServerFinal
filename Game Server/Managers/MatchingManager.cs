namespace TicTacToeGameServer.Managers
{
    public class MatchingManager
    {
        public Dictionary<string, MatchData> _allMatchesData { get; private set; }

        public MatchingManager()
        {
            _allMatchesData = new Dictionary<string, MatchData>();
        }

        public bool AddToMatchingData(string matchId,MatchData data)
        {
            if (matchId == null || data == null) 
            {
                return false;
            }
            if (_allMatchesData.ContainsKey(matchId))
                _allMatchesData[matchId] = data;
            else _allMatchesData.Add(matchId, data);
            return true;    
        }

        public void RemoveFromMatchingData(string matchId)
        {
            if (_allMatchesData.ContainsKey(matchId))
                _allMatchesData.Remove(matchId);    
        }

        public MatchData GetMatchData(string matchId)
        {
            if (_allMatchesData.ContainsKey(matchId))
                return _allMatchesData[matchId];
            return null;
        }
    }
}
