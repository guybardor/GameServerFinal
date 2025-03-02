using System.Diagnostics;
using WebSocketSharp.Server;

namespace TicTacToeGameServer.Models
{
    public class User
    {
        public enum UserState
        {
            Idle = 3001, Matching = 3002, PrePlay = 3003, Playing = 3004, Disconnected = 3005
        };

        public enum CloseCode
        {
            Unknown = 2001, DuplicateConnection = 2002, MissingUserId = 2003
        };

        private string _userId;

        public string UserId { get => _userId;}

        private IWebSocketSession _session;
        public IWebSocketSession Session { get => _session; }

        private UserState _curUserState;
        public UserState CurUserState { get => _curUserState; set => _curUserState = value; }

        private string _matchId;

        public string MatchId { get => _matchId; set => _matchId = value; }

        private DateTime _matchingDate;
        public DateTime MatchingDate { get => _matchingDate; }

        public User(string userId,IWebSocketSession session)
        {
            _userId = userId;
            _session = session;
            _curUserState = UserState.Idle;
        }

        public void SetMatchingState()
        {
            _matchingDate = DateTime.Now;
            _curUserState = UserState.Matching;
        }

        public WebSocketSharp.WebSocketState GetConnectionState()
        {
            return Session.ConnectionState;
        }
        public void CloseConnection(CloseCode _CloseCode, string _Message)
        {
            _session.Context.WebSocket.Close((ushort)_CloseCode, _Message);
        }

        public bool IsLive() {return Session != null && Session.ConnectionState == WebSocketSharp.WebSocketState.Open;}
    
        public void SendMessage(string message)
        {
            Console.WriteLine("we are send Message");
            try
            {
                if (IsLive())
                    Session.Context.WebSocket.Send(message);
                else {
                    Console.WriteLine("WebSocket is not open, reconnecting...");
                    Console.WriteLine("Socket is not open, UserId: " + UserId);
                }
                
            }
            catch (Exception ex)
            { }
        }
    }
}
