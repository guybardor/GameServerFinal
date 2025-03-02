using System.Collections.Concurrent;

namespace TicTacToeGameServer.Services
{
    public class IdToUserIdManager
    {
        private readonly ConcurrentDictionary<string, string> _connectionIdToUserIdMap;

        public IdToUserIdManager()
        {
            _connectionIdToUserIdMap = new ConcurrentDictionary<string, string>();
        }

        public void AddMapping(string id, string userId)
        {
            if (_connectionIdToUserIdMap.ContainsKey(id) == false)
                _connectionIdToUserIdMap.TryAdd(id, userId);
            else _connectionIdToUserIdMap[id] = userId;
        }

        public string GetUserId(string id)
        {
            if (_connectionIdToUserIdMap.ContainsKey(id))
                return _connectionIdToUserIdMap[id];
            return null;
        }
    }
}
