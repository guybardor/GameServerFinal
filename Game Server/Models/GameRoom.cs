using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private int _turnTime = 360;
        private int _timeOutTime = 35;
        private RoomTime _roomTime;

        // ✅ Added new fields from CreateTurnRoom request
        public string Name { get; private set; }
        public string Owner { get; private set; }
        public int MaxUsersCount { get; private set; }

        public int JoinedUsersCount { get; set; }

        public Dictionary<string, object> TableProperties { get; private set; }
        public int TurnTime { get; private set; }


        //JOINED
        public List<string> _playersOrder { get; private set; }
        //SUBSBRIBED
        [JsonIgnore]
        private Dictionary<string, User> _subplayersOrder { get; set; }
        [JsonIgnore]

        public Dictionary<string, User> _users ;
        [JsonIgnore]

        public Dictionary<string, User> Users { get => _users; }

        public GameRoom(string RoomId, SessionManager sessionManager, RoomsManager roomManager,
           IRandomizerService randomizerService, IDateTimeService dateTimeService, Dictionary<string, object> roomdetails, int JoinedUsersCount) // MatchData matchData
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


        public Dictionary<string, object> StopGame(User user, string winner)
        {
            //LeaveRoom(user);
            Dictionary<string, object> response = new Dictionary<string, object>()
            {
                {"Service","GameStopped"},
                {"Winner",winner},
                {"Sender",user.UserId },
                {"RoomId",user.MatchId }
            };

            string toSend = JsonConvert.SerializeObject(response);

            BroadcastToRoom(user, toSend);

            return response;
        }
        #endregion

        #region GameLoop

        public void GameLoop(User us)
        {
            if (_isRoomActive)
            {
                try
                {
                    if (_roomTime.IsCurrentTimeActive() == false)
                        ChangeTurn();

                    if (_isDestroyThread && _roomTime.IsRoomTimeOut() == false)
                    {
                        Console.WriteLine("Destroying room");
                        LeaveRoom(us);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("GameLoop:Error: " + ex.ToString());
                    LeaveRoom(us);
                }
            }
        }

        #endregion

        #region Logic
        public void StartGame()
        {
            Console.WriteLine("Start Game Gameserver");
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
            _isRoomActive = true;
            _isDestroyThread = true;
            _roomTime.ResetTimer();
            string toSend = JsonConvert.SerializeObject(sendData);
            //להוציא את השחקן הראשון 
            User FirstUser = _users.FirstOrDefault().Value;

            BroadcastToStartGame(FirstUser, toSend);
            //    { "Sender",   this },
            //{ "RoomId",   _data["RoomId"] },
            //{ "NextTurn", _data["NextTurn"] },
            //{ "TurnsList", _data["TurnsList"] },
            //{ "TurnTime", _data["TurnTime"] }




        }



        public void LeaveRoom(User us)
        {
            if (us != null)
            {

                Console.WriteLine("Closed Room " + DateTime.UtcNow.ToShortTimeString());
                bool joinSuccess = _users.Remove(us.UserId);
                bool SubSuccess = _subplayersOrder.Remove(us.UserId);
                bool PlayerorderSuccess = _playersOrder.Remove(us.UserId);

                bool Success = joinSuccess && SubSuccess && PlayerorderSuccess;
                if (Success)
                {
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    result.Add("Response", "LeaveRoom");
                    result.Add("IsSuccess", Success);
                    result.Add("RoomId", this.RoomId);
                    result.Add("Owner", this.Owner);
                    result.Add("MaxUsers", this.MaxUsersCount);
                    result.Add("Name", this.Name);
                    string toSend = JsonConvert.SerializeObject(result);
                    us.SendMessage(toSend);
                    BroadcastToRoom(us, toSend);//add this
                }
                if (this._users.Count == 0)
                {
                    this._isRoomActive = false;
                    this.Owner = "-1";

                }
                else if (us.UserId == this.Owner) 
                { 
                
                    this.Owner = this._users.Values.FirstOrDefault().UserId;
                }
               
              
                /* _roomManager.RemoveRoom(RoomId);*/
               
            }

        }

        public void ChangeTurn()
        {
            PassTurn();
            /*Dictionary<string, object> notifyData = new Dictionary<string, object>()
            {
                { "Service","PassTurn"},
                { "CP",_playersOrder[_turnIndex]},
                { "MC",_moveCounter}
            }; 
            
            string toSend = JsonConvert.SerializeObject(notifyData);
            BroadcastToRoom(toSend);
            return notifyData;*/
        }

        private void PassTurn()
        {
            _moveCounter++;
            _turnIndex = _turnIndex == 0 ? 1 : 0;
            _roomTime.ResetTimer();
        }

        public Dictionary<string, object> ReceivedMove(User curUser, string boardIndex)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            if (_playersOrder[_turnIndex] == curUser.UserId)
            {
                PassTurn();
                response = new Dictionary<string, object>()
                {
                    {"Service","BroadcastMove" },
                    {"Sender",curUser.UserId},
                    {"MoveData", boardIndex},
                    {"NextTurn",_playersOrder[_turnIndex] },
                    {"MC", _moveCounter },
                    {"RoomId",curUser.MatchId }
                };


               
                string toSend = JsonConvert.SerializeObject(response);
                string userid = _playersOrder[_turnIndex];
                User user = _users[userid];
                if (user != null) 
                {
                    user.SendMessage(toSend);
                }
                

                Dictionary<string, object> moveMessage = new Dictionary<string, object>()
                {
           
                    {"Sender", curUser.UserId},
                    {"MoveData", boardIndex},
                    {"MatchId", curUser.MatchId}
                };
                Console.WriteLine("ReceivedMove");
                string string_MoveMessage = JsonConvert.SerializeObject(moveMessage);  
                BroadcastToRoom(curUser, string_MoveMessage);

                /* this.alertSubUsersMove(user, boardIndex);*/
            }
            else response.Add("ErrorCode", GlobalEnums.ErrorCodes.NotPlayerTurn);

            return response;
        }
        public void alertSubUsersMove(User curUser, string boardIndex)
        {
            Dictionary<string, object> responseForSub = new Dictionary<string, object>()
            {

                {"Sender",curUser.UserId},
                {"MoveData", boardIndex},
                {"RoomId",curUser.MatchId }
            };
            string toSend = JsonConvert.SerializeObject(responseForSub);

            foreach (string userId in _subplayersOrder.Keys)
            {
                if (curUser.UserId != _subplayersOrder[userId].UserId)
                {

                    _subplayersOrder[userId].SendMessage(toSend);
                }
            }
        }
        #endregion

        

        public void BroadcastToStartGame(User user, string toSend)
        {
            foreach (string userId in _users.Keys)
            {

                Console.WriteLine("BroadcastToStartGame");

                _users[userId].SendMessage(toSend);


            }

            foreach (string userid in _subplayersOrder.Keys)
            {
                _subplayersOrder[userid].SendMessage(toSend);
            }


        }


      

        public void BroadcastToRoom(User user, string message)
        {

            Console.WriteLine("BroadcastToRoom");
            Dictionary<string, object> broadcastData = new Dictionary<string, object>()
            {
                {"Service","SendMessage"},
                {"Sender",user.UserId},
                {"MatchId",user.MatchId},
                {"Message",message}
            };
            string toSend = JsonConvert.SerializeObject(broadcastData);


           

            foreach (string userId in _subplayersOrder.Keys )
            {
                if (user.UserId != userId)
                {
                    if (_users.ContainsKey(userId))
                    {
                        
                        _users[userId].SendMessage(toSend);
                    }
                    else if (_subplayersOrder.ContainsKey(userId))
                    {
                        
                        _subplayersOrder[userId].SendMessage(toSend);
                    }
                }
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
            BroadcastToRoom(user, toSend);
        }

        public bool AddUser(string UserId, User user)
        {

            if (_users.Count >= MaxUsersCount || UserId == null || user == null)
                return false;

            if (_users.ContainsKey(UserId))
            {

                _users[UserId] = user;
            }
            else
            {
                _users.Add(UserId, user);

            }

            if (this.Owner == "-1")
            {
                this.Owner = UserId;
            }

            //לעשות מילון של ולהחזיר לו  USERJOINROOM
            string message = "user : " + UserId + " " + "has join room : " + this.RoomId;
            BroadcastToRoom(user, message);

            /* SubscribeToRoom(UserId, user);*/
            if (_playersOrder.Contains(UserId))
            {
                _playersOrder.Remove(UserId);
                _playersOrder.Add(UserId);

            }
            else
            {
                _playersOrder.Add(UserId);

            }

         
            return true;
        }





        public bool SubscribeToRoom(string UserId, User user)
        {
            if (_subplayersOrder == null || UserId == null || user == null)
                return false;

            if (_subplayersOrder.ContainsKey(UserId))
            {
                _subplayersOrder[UserId] = user;
            }
            else
                _subplayersOrder.Add(UserId, user);
            string message = "user : " + UserId + " " + "has Subscribe To room : " + this.RoomId;
            BroadcastToRoom(user, message);
            return true;
        }

    }
}


/* BrodcastRoom 322 

HashSet<string> allRecipients = new HashSet<string>(_users.Keys);
            foreach (string subUserId in _subplayersOrder.Keys)
            {
                allRecipients.Add(subUserId);
            }*/

/* private void BroadcastToRoom(User user, string toSend)
       {
           foreach (string userId in _users.Keys)
           {

               if (user.UserId != _users[userId].UserId)
               {

                   _users[userId].SendMessage(toSend);
               }

           }
           foreach (string userId in _subplayersOrder.Keys)
           {
               if (user.UserId != _subplayersOrder[userId].UserId)
               {

                   _subplayersOrder[userId].SendMessage(toSend);
               }
           }




       }
*/



/*public void StartGameOnlyForFirstUser()
      {
          // נוודא שיש לנו לפחות משתמש אחד
          if (_playersOrder.Count == 0)
              return;

          // נאתר את המשתמש הראשון ברשימת המשחק
          string firstUserId = _playersOrder[0];
          if (!_users.ContainsKey(firstUserId))
              return;

          User firstUser = _users[firstUserId];

          // בונים את המידע הנשלח
          Dictionary<string, object> sendData = new Dictionary<string, object>()
  {
      { "Service","StartGame"},
      { "RoomId",RoomId},
      { "TT",_dateTimeService.GetUtcTime()},
      { "TurnTime",_turnTime},
      { "NextTurn",_playersOrder[_turnIndex]},
      { "TurnsList",_playersOrder},
      { "MC",_moveCounter},
      { "Sender",this.Owner }
  };

          // הופכים את המידע ל־JSON
          string toSend = JsonConvert.SerializeObject(sendData);

          // שולחים אך ורק למשתמש הראשון
          firstUser.SendMessage(toSend);

          // מעדכנים שהחדר פעיל (אופציונלי, בהתאם לזרימת התוכנה שלך)
          _isRoomActive = true;
          _isDestroyThread = true;
          _roomTime.ResetTimer();
      }*/


/*if (this.Owner == userId)
            {

                //_users[userId].SendMessage(toSend);
            }*/



//StartGameOnlyForFirstUser();



//string firstPlayerId = _playersOrder[0];
//User firstPlayer = _users[firstPlayerId];


//string jsonMsg = JsonConvert.SerializeObject(data);
//firstPlayer.SendMessage(jsonMsg);

// אפשר גם (אופציונלי) להתחיל את המשחק בבת־אחת לכולם