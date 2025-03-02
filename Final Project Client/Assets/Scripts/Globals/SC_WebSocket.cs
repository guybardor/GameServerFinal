using BestHTTP.SecureProtocol.Org.BouncyCastle.Math.EC.Multiplier;
using BestHTTP.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

public class SC_WebSocket : MonoBehaviour
{
    public string serverUrl = "ws://localhost:8080/gameserver/game/";
    public delegate void GeneralHandler(Dictionary<string, object> _PassedVariables);
    public static event GeneralHandler OnConnect;
    public static event GeneralHandler OnDisconnect;
    public static event GeneralHandler OnTextMessage;
    public static event GeneralHandler OnBinaryMessage;
    public static event GeneralHandler OnErrorReceived;

    public static event GeneralHandler OnRegister;
    public static event GeneralHandler OnSendMessage;
    public static event GeneralHandler OnBroadcastMessage;
    public static event GeneralHandler OnReadyToPlay;
    public static event GeneralHandler OnPassTurn;

    public static Action<bool,string> OnCreateTurnRoom;
    public static Action<bool, string> OnJoinRoom;
    public static Action<bool, string> OnSubscribeRoom; 
    public static Action<List<RoomData>> OnGetRoomsInRange;
    public static Action<RoomData, string> OnUserJoinRoom;
    public static Action<RoomData, List<string>,Dictionary<string,object>> OnGetLiveRoomInfo;
    public static Action<string,string,string,int,List<string>> OnStartGame;
    public static Action<bool,string> OnCancelMatching;
    public static Action<Move> OnBroadcastMove;
    public static Action<string, string> OnGameStopped;
    public static Action<bool, LeaveRoomData> OnLeaveRoom;
    public static Action<string, string, string> OnSendChat;

    private string userId = "";
    private WebSocket webSocket;

    #region Singleton

    static SC_WebSocket instance;
    public static SC_WebSocket Instance
    {
        get
        {
            if (instance == null && GameObject.Find("SC_WebSocket") != null)
                instance = GameObject.Find("SC_WebSocket").GetComponent<SC_WebSocket>();

            return instance;
        }
    }

    #endregion

    #region Server Callbacks
    private void OnWebSocketOpen(WebSocket webSocket)
    {
        Debug.Log("<color=green>Response: </color>WebSocket is now Open!");
        Dictionary<string, object> _data = new Dictionary<string, object>();
        _data.Add("IsOpen",true);
        if (OnConnect != null)
            OnConnect(_data);
    }
    private void OnMessageReceived(WebSocket webSocket, string message)
    {
        Debug.Log("<color=green>Response: " + message + "</color>");
        try
        {
            Dictionary<string, object> _data = MiniJSON.Json.Deserialize(message) as Dictionary<string, object>;
            bool isSuccess = false;
            string roomId = string.Empty;
            string errorMessage = string.Empty;

            if (_data.ContainsKey("Response"))
            {
                switch (_data["Response"].ToString())
                {
                    case "Register":
                        if (OnRegister != null)
                            OnRegister(_data);
                        break;
                    case "CancelMatching":
                        if (_data.ContainsKey("IsSuccess"))
                            isSuccess = bool.Parse(_data["IsSuccess"].ToString());
                        if (_data.ContainsKey("ErrorCode"))
                            errorMessage = _data["ErrorCode"].ToString();

                        if (OnCancelMatching != null)
                            OnCancelMatching(isSuccess, errorMessage);
                        break;
                    case "SendMessage":
                        if (OnSendMessage != null)
                            OnSendMessage(_data);
                        break;
                    case "GetRoomsInRange":
                        List<RoomData> roomsData = new List<RoomData>();
                        if (_data.ContainsKey("Rooms"))
                        {
                            List<object> rooms = (List<object>)_data["Rooms"];
                            foreach (object room in rooms)
                            {
                                Dictionary<string, object> roomDictionary = (Dictionary<string, object>)room;
                                roomsData.Add(RoomData.Create(roomDictionary));
                            }
                        }
                        if (OnGetRoomsInRange != null)
                            OnGetRoomsInRange(roomsData);

                        break;
                    case "CreateTurnRoom":
                        if(_data.ContainsKey("RoomId") && _data.ContainsKey("IsSuccess"))
                        {
                            isSuccess = bool.Parse(_data["IsSuccess"].ToString());
                            roomId = _data["RoomId"].ToString();
                        }

                        if (OnCreateTurnRoom != null)
                            OnCreateTurnRoom(isSuccess, roomId);
                        break;
                    case "JoinRoom":
                        if (_data.ContainsKey("RoomId") && _data.ContainsKey("IsSuccess"))
                        {
                            isSuccess = bool.Parse(_data["IsSuccess"].ToString());
                            roomId = _data["RoomId"].ToString();
                        }
                        if (OnJoinRoom != null)
                            OnJoinRoom(isSuccess, roomId);
                        break;
                    case "SubscribeRoom":
                        if (_data.ContainsKey("RoomId") && _data.ContainsKey("IsSuccess"))
                        {
                            isSuccess = bool.Parse(_data["IsSuccess"].ToString());
                            roomId = _data["RoomId"].ToString();
                        }
                        if (OnSubscribeRoom != null)
                            OnSubscribeRoom(isSuccess, roomId);
                        break;
                    case "LeaveRoom":
                        if (_data.ContainsKey("IsSuccess") && _data.ContainsKey("RoomId") && _data.ContainsKey("Owner") && _data.ContainsKey("MaxUsers") && _data.ContainsKey("Name"))
                        {
                            string curRoomId = _data["RoomId"].ToString();
                            string curOwner = _data["Owner"].ToString();
                            int curMaxUsers = int.Parse(_data["MaxUsers"].ToString());
                            string curName = _data["Name"].ToString();
                            isSuccess = bool.Parse(_data["IsSuccess"].ToString());

                            LeaveRoomData leaveRoomData = new LeaveRoomData(curRoomId, curOwner, curMaxUsers, curName);
                            if (OnLeaveRoom != null)
                                OnLeaveRoom(isSuccess, leaveRoomData);
                        }
                        else OnLeaveRoom(false, null);
                        break;
                    case "GetLiveRoomInfo":
                        if (_data.ContainsKey("RoomData") && _data.ContainsKey("RoomProperties") && _data.ContainsKey("Users"))
                        {
                            List<string> users = new List<string>();
                            List<object> objList = (List<object>)_data["Users"];
                            users = objList.Select(obj => obj.ToString()).ToList();

                            Dictionary<string, object> roomProperties = (Dictionary<string, object>)_data["RoomProperties"];

                            Dictionary<string, object> roomDictionary = (Dictionary<string, object>)_data["RoomData"]; ;
                            RoomData roomData = RoomData.Create(roomDictionary);
                            if (OnGetLiveRoomInfo != null)
                                OnGetLiveRoomInfo(roomData, users, roomProperties);
                        }
                        else
                        {
                            if (OnGetLiveRoomInfo != null)
                                OnGetLiveRoomInfo(null, new List<string>(), new Dictionary<string, object>());
                        }
                        break;
                }
            }

            if (_data.ContainsKey("Service"))
            {
                switch (_data["Service"].ToString())
                {
                    case "BroadcastMessage":
                        if (OnBroadcastMessage != null)
                            OnBroadcastMessage(_data);
                        break;
                    case "ReadyToPlay":
                        if (OnReadyToPlay != null)
                            OnReadyToPlay(_data);
                        break;
                    case "StartGame":
                        if (_data.ContainsKey("Sender") && _data.ContainsKey("RoomId") && _data.ContainsKey("NextTurn") 
                            && _data.ContainsKey("TurnsList") && _data.ContainsKey("TurnTime"))
                        {
                            string sender = _data["Sender"].ToString();
                            string curRoomId = _data["RoomId"].ToString();
                            string nextTurn = _data["NextTurn"].ToString();
                            int turnTime = int.Parse(_data["TurnTime"].ToString()); 
                            List<string> turnsList = ((List<object>)_data["TurnsList"]).Select(obj => obj.ToString()).ToList();

                            if (OnStartGame != null)
                                OnStartGame(sender, curRoomId, nextTurn, turnTime, turnsList);
                        }
                        else Debug.Log("StartGame Missing Variables from the server");
                        break;
                    case "BroadcastMove":
                        if (_data.ContainsKey("Sender") && _data.ContainsKey("RoomId") && _data.ContainsKey("NextTurn")
                           && _data.ContainsKey("MoveData"))
                        {
                            string sender = _data["Sender"].ToString();
                            string curRoomId = _data["RoomId"].ToString();
                            string nextTurn = _data["NextTurn"].ToString();
                            string moveData = _data["MoveData"].ToString();
                            Move curMoveData = new Move(sender, curRoomId, nextTurn, moveData);

                            if (OnBroadcastMove != null)
                                OnBroadcastMove(curMoveData);
                        }
                        break;
                    case "PassTurn":
                            if (OnPassTurn != null)
                                OnPassTurn(_data);
                        break;
                    case "GameStopped":
                        if (_data.ContainsKey("Sender") && _data.ContainsKey("RoomId"))
                        {
                            string sender = _data["Sender"].ToString();
                            string curRoomId = _data["RoomId"].ToString();

                            if (OnGameStopped != null)
                                OnGameStopped(sender, curRoomId);
                        }
                        break;

                    case "UserJoinRoom":
                       
                        if (_data.ContainsKey("RoomData") && _data.ContainsKey("UserId"))
                        {
                            string userId = _data["UserId"].ToString();
                            Dictionary<string, object> sentRoomData = (Dictionary<string, object>)_data["RoomData"];
                            RoomData _curRoomData = RoomData.Create(sentRoomData);

                            if (OnUserJoinRoom != null)
                                OnUserJoinRoom(_curRoomData, userId);
                        }
                        else
                        {
                            if (OnUserJoinRoom != null)
                                OnUserJoinRoom(null, string.Empty);
                        }
                        break;
                    case "SendChat":
                        if (_data.ContainsKey("Message") && _data.ContainsKey("RoomId") && _data.ContainsKey("Sender"))
                        {
                            string curMessage = _data["Message"].ToString();
                            string curRoomId = _data["RoomId"].ToString();
                            string curSender = _data["Sender"].ToString();
                            if (OnSendChat != null)
                                OnSendChat(curSender, curRoomId, curMessage);
                        }
                        else
                        {
                            if (OnSendChat != null)
                                OnSendChat(string.Empty, string.Empty, string.Empty);
                        }
                        break; 
                }
            }
        }
        catch (System.Exception e)
        {
           
            Debug.LogError(e.Message);
        }
    }
    private void OnBinaryMessageReceived(WebSocket webSocket, byte[] message)
    {
        Debug.Log("Binary Message received from server. Length: " + message.Length);
        Dictionary<string, object> _data = new Dictionary<string, object>();
        _data.Add("DataMessage", message);
        if (OnBinaryMessage != null)
            OnBinaryMessage(_data);
    }
    private void OnWebSocketClosed(WebSocket webSocket, System.UInt16 code, string message)
    {
        Debug.Log("WebSocket is now Closed!");
        Dictionary<string, object> _data = new Dictionary<string, object>();
        _data.Add("CloseMessage", message);
        if (OnDisconnect != null)
            OnDisconnect(_data);
    }
    void OnError(WebSocket ws, string error)
    {
        Debug.LogError("Error: " + error);
        Debug.LogError("Ws Open? " + ws.IsOpen);
        Dictionary<string, object> _data = new Dictionary<string, object>();
        _data.Add("IsConnected", ws.IsOpen);
        _data.Add("Error", error);

        if (OnErrorReceived != null)
            OnErrorReceived(_data);
    }

    #endregion

    #region Server Calls

    public void SetGameServerInfo(string _ConnectionUrl, string _UserId)
    {
        serverUrl = _ConnectionUrl;
        userId = _UserId;
    }
    public void OpenConnection()
    {
        if(userId != string.Empty)
            ConnectServer(userId);
    }
    private bool SendMessage(Dictionary<string, object> _Data)
    {
        try
       {
            if (webSocket.IsOpen)
            {
                string _toSend = MiniJSON.Json.Serialize(_Data);
                webSocket.Send(_toSend);
            }
        }
        catch (System.Exception e) { return false; }
        return true;
    }

    private void ConnectServer(string _UserId)
    {
        Dictionary<string, object> _data = new Dictionary<string, object>();
        _data.Add("UserId", _UserId);
        string _toSend = MiniJSON.Json.Serialize(_data);

        Debug.Log("<color=blue>ConnectServer: </color>" + serverUrl + _toSend);
        webSocket = new WebSocket(new Uri(serverUrl + "?data=" + _toSend));
        webSocket.OnOpen += OnWebSocketOpen;
        webSocket.OnMessage += OnMessageReceived;
        webSocket.OnBinary += OnBinaryMessageReceived;
        webSocket.OnClosed += OnWebSocketClosed;
        webSocket.OnError += OnError;

        webSocket.Open();
    }
    public void CloseConnection()
    {
        if (webSocket != null && webSocket.IsOpen)
            webSocket.Close();
    }
    public bool IsOpen()
    {
        if (webSocket != null)
            return webSocket.IsOpen;
        return false;
    }

    public void GetRoomsInRange(int _MinUserCount, int _MaxUserCount)
    {
        Debug.Log("<color=yellow> Service: " + "GetRoomsInRange" + "</color>");
        Dictionary<string, object> _data = new Dictionary<string, object>()
        {
            {"Service", "GetRoomsInRange"},
            {"MinUserCount", _MinUserCount},
            {"MaxUserCount", _MaxUserCount}
        };
        SendMessage(_data);
    }
    public void CreateTurnRoom(string _Name, string _Owner, int _MaxUsers, Dictionary<string, object> _TableProperties, int _TurnTime)
    {
        Debug.Log("<color=yellow> Service: " + "CreateTurnRoom" + "</color>");
        Dictionary<string, object> _data = new Dictionary<string, object>()
        {
            {"Service", "CreateTurnRoom"},
            {"Name", _Name},
            {"Owner", _Owner},
            {"MaxUsers", _MaxUsers},
            {"TableProperties", _TableProperties},
            {"TurnTime", _TurnTime},
        };
        SendMessage(_data);
    }
    public void JoinRoom(string roomId)
    {
        Debug.Log("<color=yellow> Service: " + "JoinRoom" + "</color>");
        Dictionary<string, object> _data = new Dictionary<string, object>()
        {
            {"Service", "JoinRoom"},
            {"RoomId", roomId}
        };
        SendMessage(_data);
    }
    public void SubscribeRoom(string roomId)
    {

        Debug.Log("<color=yellow> Service: " + "SubscribeRoom" + "</color>");
        Dictionary<string, object> _data = new Dictionary<string, object>()
        {
            {"Service", "SubscribeRoom"},
            {"RoomId", roomId}
        };
        SendMessage(_data);
    }
    public void GetLiveRoomInfo(string roomId)
    {
        Debug.Log("<color=yellow> Service: " + "GetLiveRoomInfo" + "</color>");
        Dictionary<string, object> _data = new Dictionary<string, object>()
        {
            { "Service", "GetLiveRoomInfo"},
            { "RoomId", roomId},
        };
        SendMessage(_data);
    }
    public void StartGame() { SendMessage(new Dictionary<string, object>() { { "Service", "StartGame" } }); }
    public void CancelMatching()
    {
        SendMessage(new Dictionary<string, object>() { { "Service", "CancelMatching" } });
    }

    public void SendServerMessage(Dictionary<string, object> _Data)
    {
        _Data.Add("Service", "SendMessage");
        SendMessage(_Data);
    }
    public void SendMove(string _Index)
    {
        Dictionary<string, object> _data = new Dictionary<string, object>()
        {
            { "Service", "SendMove"},
            { "MoveData", _Index }
        };
        SendMessage(_data);
    }
    public void StopGame(string _Winner)
    {
        Dictionary<string, object> _data = new Dictionary<string, object>()
        {
            {"Service", "StopGame"},
            {"Winner", _Winner},
        };
        SendMessage(_data);
    }

    public void LeaveRoom(string roomId)
    {
        Dictionary<string, object> _data = new Dictionary<string, object>()
        {
            { "Service", "LeaveRoom"},
            { "RoomId", roomId }
        };
        SendMessage(_data);
    }
    public void SendChat(string roomId,string message)
    {
        Dictionary<string, object> _data = new Dictionary<string, object>()
        {
            { "Service", "SendChat"},
            { "RoomId", roomId },
            { "Message", message }
        };
        SendMessage(_data);
    }

    #endregion

}
