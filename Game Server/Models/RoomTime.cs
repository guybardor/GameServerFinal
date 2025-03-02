namespace TicTacToeGameServer.Models
{
    public class RoomTime
    {
        private float _maxTurnTime;
        private float _timeOutTime;
        private DateTime _startDate;
        private DateTime _startRoomDate;
        public RoomTime(float maxTurnTime,float timeOutTime)
        {
            _maxTurnTime = maxTurnTime;
            _timeOutTime = timeOutTime;
            _startRoomDate = DateTime.UtcNow;
        }

        public void ResetTimer()
        {
            _startDate = DateTime.UtcNow;
        }

        public bool IsCurrentTimeActive()
        {
            if(_startDate != null)
            {
               TimeSpan diff = DateTime.UtcNow - _startDate;
                if(diff.TotalSeconds < _maxTurnTime)
                    return true;
            }
            return false;
        }

        public bool IsRoomTimeOut()
        {
            if (_startRoomDate != null)
            {
                TimeSpan diff = DateTime.UtcNow - _startRoomDate;
                if (diff.TotalSeconds < _timeOutTime)
                    return true;
            }
            return false;
        }

    }
}
