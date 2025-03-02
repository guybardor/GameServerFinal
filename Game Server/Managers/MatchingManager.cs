namespace TicTacToeGameServer.Managers
{
    public class MatchingManager
    {
        private Dictionary<string, MatchData> _allMatchesData;

        public MatchingManager()
        {
            _allMatchesData = new Dictionary<string, MatchData>();
        }

        public void AddToMatchingData(string matchId,MatchData data)
        {
            if (_allMatchesData.ContainsKey(matchId))
                _allMatchesData[matchId] = data;
            else _allMatchesData.Add(matchId, data);
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
