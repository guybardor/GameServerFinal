using TicTacToeGameServer.Interfaces;

namespace TicTacToeGameServer.Services
{
    public class DateTimeService : IDateTimeService
    {
        public string GetUtcTime()
        {
            string _date = DateTime.UtcNow.Year + "-" + DateTime.UtcNow.Month + "-" + DateTime.UtcNow.Day;
            _date += " " + DateTime.UtcNow.Hour + ":" + DateTime.UtcNow.Minute + ":" + DateTime.UtcNow.Second;
            return _date;
        }
    }
}
