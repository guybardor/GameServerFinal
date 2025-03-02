using System.Text.RegularExpressions;
using TicTacToeGameServer.Interfaces;

namespace TicTacToeGameServer.Services
{
    public class RandomizerService : IRandomizerService
    {
        public int GetRandomNumber(int min, int max)
        {
            var seed = Convert.ToInt32(Regex.Match(Guid.NewGuid().ToString(), @"\d+").Value);
            return new Random(seed).Next(min, max);
        }
    }
}
