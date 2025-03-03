using Newtonsoft.Json;
using System;
using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;

namespace TicTacToeGameServer.Models
{
    public class GameRoom
    {
        public string RoomId { get; private set; }
        [JsonIgnore]

        private SessionManager _sessionManager;
        [JsonIgnore]

        private RoomsManager _roomManager;
        private IRandomizerService _randomizerService;
        private IDateTimeService _dateTimeService;

        private bool _isRoomActive = false;
        private bool _isDestroyThread = false;
        private int _moveCounter = 0;
        private int _turnIndex = 0;
        private int _turnTime = 120;
        private int _timeOutTime = 35;
        private RoomTime _roomTime;

        // ✅ Added new fields from CreateTurnRoom request
        public string Name { get; private set; }
        public string Owner { get; private set; }
        public int MaxUsersCount { get; private set; }

        public int JoinedUsersCount { get;  set; }

        public Dictionary<string, object> TableProperties { get; private set; }
        public int TurnTime { get; private set; }


        //JOINED
        public List<string> _playersOrder { get; private set; }
        //SUBSBRIBED
        [JsonIgnore]
        private Dictionary<string,User> _subplayersOrder { get;  set; }
        [JsonIgnore]

        private Dictionary<string,User> _users;
        [JsonIgnore]

        public Dictionary<string, User> Users { get => _users; }

        public GameRoom(string RoomId, SessionManager sessionManager, RoomsManager roomManager,
           IRandomizerService randomizerService, IDateTimeService dateTimeService, Dictionary<string, object> roomdetails,int JoinedUsersCount) // MatchData matchData
        {

            this.RoomId = RoomId;
            _sessionManager = sessionManager;
            _roomManager = roomManager;
            _randomizerService = randomizerService;
            _dateTimeService = dateTimeService;
            _isRoomActive = false; // 🚨 Do NOT start the game yet
            _moveCounter = 0;
            _turnIndex = 0;
            _isRoomActive = true;
            Console.WriteLine($"[GameRoom!@#] Using RoomsManager Instance HashCode: {_roomManager.GetHashCode()}");
            Name = roomdetails["Name"].ToString();
            Owner = roomdetails["Owner"].ToString();
            MaxUsersCount = int.Parse(roomdetails["MaxUsers"].ToString());
            TableProperties = new Dictionary<string, object>() { { "Password", "Shenkar" } };

            //TableProperties["Password"] = roomdetails["TableProperties"].ToString() ;
            TurnTime = int.Parse(roomdetails["TurnTime"].ToString());
            _roomTime = new RoomTime(TurnTime, _timeOutTime);
            JoinedUsersCount = this.JoinedUsersCount;


            // ✅ Initialize Storage (but don't add players yet)
            _playersOrder = new List<string>();
            _subplayersOrder = new Dictionary<string, User>();
            _users = new Dictionary<string, User>();

        }

        #region Requests
        public Dictionary<string,object> ReceivedMove(User curUser,string boardIndex)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            if (_playersOrder[_turnIndex] == curUser.UserId)
            {
                PassTurn();
                response = new Dictionary<string, object>()
                {
                    {"Service","BroadcastMove" },
                    {"SenderId",curUser.UserId},
                    {"Index", boardIndex},
                    {"CP",_playersOrder[_turnIndex] },
                    {"MC", _moveCounter }
                };

                string toSend = JsonConvert.SerializeObject(response);
                BroadcastToRoom(toSend);
            }
            else response.Add("ErrorCode", GlobalEnums.ErrorCodes.NotPlayerTurn);

            return response;
        }

        public Dictionary<string, object> StopGame(User user, string winner)
        {
            CloseRoom();
            Dictionary<string, object> response = new Dictionary<string, object>()
            {
                {"Service","StopGame"},
                {"Winner",winner}
            };

            string toSend = JsonConvert.SerializeObject(response);
            BroadcastToRoom(toSend);

            return response;
        }
        #endregion

        #region GameLoop

        public void GameLoop()
        {
            if(_isRoomActive)
            {
                try
                {
                    if (_roomTime.IsCurrentTimeActive() == false)
                        ChangeTurn();

                    if(_isDestroyThread && _roomTime.IsRoomTimeOut() == false)
                    {
                        Console.WriteLine("Destroying room");
                        CloseRoom();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("GameLoop:Error: " + ex.ToString());   
                    CloseRoom();
                }
            }
        }

        #endregion

        #region Logic
        public void StartGame()
        {
            _turnIndex = _randomizerService.GetRandomNumber(0, 1);

            Dictionary<string, object> sendData = new Dictionary<string, object>()
            {
                { "Service","StartGame"},
                { "RoomId",RoomId},
                { "TT",_dateTimeService.GetUtcTime()},
                { "TurnTime",_turnTime},
                { "NextTurn",_playersOrder[_turnIndex]},
                { "TurnsList",_playersOrder},
                { "MC",_moveCounter},
                {"Sender",this.Owner }
            };


        //    { "Sender",   this },
        //{ "RoomId",   _data["RoomId"] },
        //{ "NextTurn", _data["NextTurn"] },
        //{ "TurnsList", _data["TurnsList"] },
        //{ "TurnTime", _data["TurnTime"] }

            string toSend = JsonConvert.SerializeObject(sendData);
            BroadcastToRoom(toSend);

            _isRoomActive = true;
            _isDestroyThread = true;
            _roomTime.ResetTimer();
        }

        private void PassTurn()
        {
            _moveCounter++;
            _turnIndex = _turnIndex == 0 ? 1 : 0;
            _roomTime.ResetTimer();
        }

        private void CloseRoom()
        {
            Console.WriteLine("Closed Room " + DateTime.UtcNow.ToShortTimeString());
           _isRoomActive = false;
            _roomManager.RemoveRoom(RoomId);
        }

        private void ChangeTurn()
        {
            PassTurn();
            Dictionary<string, object> notifyData = new Dictionary<string, object>()
            {
                { "Service","PassTurn"},
                { "CP",_playersOrder[_turnIndex]},
                { "MC",_moveCounter}
            }; 
            
            string toSend = JsonConvert.SerializeObject(notifyData);
            BroadcastToRoom(toSend);
        }

        #endregion

        private void BroadcastToRoom(string toSend)
        {
            foreach (string userId in _users.Keys)
            {
                _users[userId].SendMessage(toSend);

                if (this.Owner == userId)
                {
                   
                //_users[userId].SendMessage(toSend);
                }
            }
            foreach (string userId in _subplayersOrder.Keys)
                _subplayersOrder[userId].SendMessage(toSend);
            
                


        }

        private void BroadcastToRoomV2(User user, string message)
        {
            Dictionary<string, object> broadcastData = new Dictionary<string, object>()
            {
                {"Service","SendMessage"},
                {"Sender",user.UserId},
                {"MatchId",user.MatchId},
                {"Message",message}
            };
            string toSend = JsonConvert.SerializeObject(broadcastData);
            

            foreach (string userId in _users.Keys)
            {
              //  SendChat(user, message);
               _users[userId].SendMessage(toSend);
            }
            foreach (string userId in _subplayersOrder.Keys)
            {
              //  SendChat(user, message);

                _subplayersOrder[userId].SendMessage(toSend);
            }




        }

        public void SendChat(User user, string message)
        {
            Dictionary<string, object> broadcastData = new Dictionary<string, object>()
            {
                {"Service","SendMessage"},
                {"Sender",user.UserId},
                {"MatchId",user.MatchId},
                {"Message",message}
            };

            string toSend = JsonConvert.SerializeObject(broadcastData);
            BroadcastToRoom(toSend);
        }

        public bool AddUser(string UserId ,  User user)
        {
            if (_users.Count >= MaxUsersCount || UserId == null || user == null)
                return false;

            if (_users.ContainsKey(UserId))
                return false;

            _users.Add(UserId, user);
            BroadcastToRoomV2(user,"user : " + UserId + " " + "has join room : " + this.RoomId);
           /* SubscribeToRoom(UserId, user);*/
            _playersOrder.Add(UserId);









            if (_users.Count == 2)
            {
                //string firstPlayerId = _playersOrder[0];
                //User firstPlayer = _users[firstPlayerId];

           
                //string jsonMsg = JsonConvert.SerializeObject(data);
                //firstPlayer.SendMessage(jsonMsg);

                // אפשר גם (אופציונלי) להתחיל את המשחק בבת־אחת לכולם
                 StartGame(); 
            }








            return true;
        }

        public bool SubscribeToRoom(string UserId, User user)
        {
            if (_subplayersOrder == null ||  UserId == null || user == null || _subplayersOrder.ContainsKey(UserId))
                return false;

            _subplayersOrder.Add(UserId, user);


           BroadcastToRoomV2(user,"user : " + UserId + " " + "has Subscribe To room : " + this.RoomId);
            return true;
        }

    }
}
