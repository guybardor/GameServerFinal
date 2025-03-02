using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Managers
{
    public class SessionManager
    {
        private Dictionary<string, User> _userSessions;

        public SessionManager()
        {
            _userSessions = new Dictionary<string, User>();
        }

        public void AddUser(User user)
        {
            if (user == null)
                return;

            if (_userSessions.ContainsKey(user.UserId))
            {
                User storedUser = _userSessions[user.UserId];
                if (storedUser.GetConnectionState() == WebSocketSharp.WebSocketState.Open)
                    storedUser.CloseConnection(User.CloseCode.DuplicateConnection,"Closed Old Connection");

                _userSessions.Remove(user.Session.ID);
                _userSessions[user.UserId] = user;
            }
            else _userSessions.Add(user.UserId, user);

            if (_userSessions.ContainsKey(user.Session.ID))
                _userSessions[user.Session.ID] = user;
            else _userSessions.Add(user.Session.ID, user);
        }

        public User GetUser(string Id)
        {
            if(_userSessions != null && _userSessions.ContainsKey(Id))
                return _userSessions[Id];
            return null;
        }

        public void UpdateUser(User user)
        {
            if (user == null)
                return;

            if (_userSessions.ContainsKey(user.UserId))
                _userSessions[user.UserId] = user;

            if (_userSessions.ContainsKey(user.Session.ID))
                _userSessions[user.Session.ID] = user;
        }
    }
}
