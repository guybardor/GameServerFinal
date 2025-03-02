using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Managers
{
    public class SearchingManager
    {
        private Dictionary<string, int> _searchingManager;

        public SearchingManager() 
        {
            _searchingManager = new Dictionary<string, int>();
        }

        public void AddToSearch(string userId,int rating)
        {
            if(_searchingManager.ContainsKey(userId))
                _searchingManager[userId] = rating;
            else _searchingManager.Add(userId, rating);
        }
        public void RemoveToSearch(string userId)
        {
            if (_searchingManager.ContainsKey(userId))
                _searchingManager.Remove(userId);
        }
        public List<SearchData> GetSearchingList()
        {
            List<SearchData> searchingData = new List<SearchData>();
            foreach (string userId in _searchingManager.Keys)
                searchingData.Add(new SearchData(userId, _searchingManager[userId]));
            return searchingData;
        }
    }
}
