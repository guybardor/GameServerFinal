using LobbyServer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace LobbyServer.Services
{
    public class RedisBaseService : IRedisBaseService
    {
        private readonly IDatabase _database;

        //public RedisBaseService(IConfiguration configuration, ConnectionMultiplexer connectionMultiplexer)
        public RedisBaseService(IConfiguration configuration)
        {
            string connectionString = configuration["Redis:ConnectionString"].ToString();
            _database = ConnectionMultiplexer.Connect(connectionString).GetDatabase();
        }

        public string GetString(string key) => _database.StringGet(key);

        public void SetString(string key, string value) => _database.StringSet(key, value);

        public Dictionary<string, string> GetDictionary(string key)
        {
            var result = new Dictionary<string, string>();
            var data = _database.HashGetAll(key);
            foreach (var entry in data)
            {
                result[entry.Name] = entry.Value;
            }
            return result;
        }

        public void SetDictionary(string key, Dictionary<string, string> data)
        {
            var entries = data.Select(d => new HashEntry(d.Key, d.Value)).ToArray();
            _database.HashSet(key, entries);
        }

        public void RemoveKey(string key) => _database.KeyDelete(key);
    }
}
