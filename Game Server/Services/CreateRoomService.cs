using System;
using System.Collections.Generic;
using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Managers;
using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Services
{
    public class CreateRoomService : ICreateRoomService
    {
        private readonly IMatchIdRedisService _matchIdRedisService;
        private readonly RoomsManager _roomsManager;
        private readonly SessionManager _sessionManager;
        private readonly IRandomizerService _randomizerService;
        private readonly IDateTimeService _dateTimeService;
        private readonly MatchingManager _matchingManager;

        public CreateRoomService(IMatchIdRedisService matchIdRedisService, 
            RoomsManager roomsManager, SessionManager sessionManager,
            IRandomizerService randomizerService, IDateTimeService dateTimeService , MatchingManager _matchingManager) 
        {
            
            _matchIdRedisService = matchIdRedisService;
            _roomsManager = roomsManager;
            _sessionManager = sessionManager;
            _randomizerService = randomizerService;
            _dateTimeService = dateTimeService;
            _matchingManager = _matchingManager;
           
        }

        public Dictionary<string, object> Create(User user , Dictionary<string,object> Matchdeatails)
        {


            return CreateRoom(user, Matchdeatails);

            
        }

        private Dictionary<string,object> CreateRoom(User user,Dictionary<string, object> Matchdeatails) //MatchData curMatchData
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            bool IsSuccess = false;
            dic.Add("Response", "CreateTurnRoom");
            dic.Add("IsSuccess", IsSuccess);
            try
            {
                
               
                int dbMatchId = 1;
                string redisMatchId = _matchIdRedisService.GetMatchId();
                if (redisMatchId != null && redisMatchId != string.Empty) 
                {
                    dbMatchId = int.Parse(redisMatchId) + 1;
                }

                

                _matchIdRedisService.SetMatchId(dbMatchId.ToString());

                if(_roomsManager.IsRoomExist(dbMatchId.ToString()) == false && _roomsManager.ActiveRooms != null)
                {
                    GameRoom gameRoom = new GameRoom(dbMatchId.ToString(), _sessionManager, _roomsManager, _matchingManager,
                        _randomizerService, _dateTimeService, Matchdeatails,0);

                    dic["IsSuccess"] = _roomsManager.AddRoom(dbMatchId.ToString(), gameRoom);
                    if (bool.Parse(dic["IsSuccess"].ToString()) == true) 
                    {

                        
                        dic.Add("RoomId", dbMatchId.ToString());


                    }
                   
                    //gameRoom.StartGame();
                }

                
            }
            catch (Exception e) { }

            return dic;
        }
    }
}
