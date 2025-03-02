using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_LobbyLogic : MonoBehaviour
{
    public Dictionary<string, GameObject> unityObjects;
    private int minUsersCound = 1;
    private int maxUsersCound = 2;
    private int turnTime = 30;

    private List<RoomData> _roomsData;
    private int _roomsIndex = 0;
    private string _roomId = string.Empty;
    private RoomData _curRoomData;
    private Dictionary<string,object> _properties;


    #region Singleton
    static SC_LobbyLogic instance;
    public static SC_LobbyLogic Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_LobbyLogic").GetComponent<SC_LobbyLogic>();

            return instance;
        }
    }
    #endregion

    #region Monobehaviour
    private void OnEnable()
    {
        SC_WebSocket.OnConnect += OnConnect;
        SC_WebSocket.OnDisconnect += OnDisconnect;
        SC_WebSocket.OnErrorReceived += OnDisconnect;
        SC_WebSocket.OnGetRoomsInRange += OnGetRoomsInRange;
        SC_WebSocket.OnCreateTurnRoom += OnCreateTurnRoom;
        SC_WebSocket.OnJoinRoom += OnJoinRoom;
        SC_WebSocket.OnUserJoinRoom += OnUserJoinRoom;
        SC_WebSocket.OnSubscribeRoom += OnSubscribeRoom;
        SC_WebSocket.OnGetLiveRoomInfo += OnGetLiveRoomInfo;
        SC_WebSocket.OnStartGame += OnStartGame;
        SC_WebSocket.OnCancelMatching += OnCancelMatching;
    }


    private void OnDisable()
    {
        SC_WebSocket.OnConnect -= OnConnect;
        SC_WebSocket.OnDisconnect -= OnDisconnect;
        SC_WebSocket.OnErrorReceived -= OnDisconnect;
        SC_WebSocket.OnGetRoomsInRange -= OnGetRoomsInRange;
        SC_WebSocket.OnCreateTurnRoom -= OnCreateTurnRoom;
        SC_WebSocket.OnJoinRoom -= OnJoinRoom;
        SC_WebSocket.OnUserJoinRoom -= OnUserJoinRoom;
        SC_WebSocket.OnSubscribeRoom -= OnSubscribeRoom;
        SC_WebSocket.OnGetLiveRoomInfo -= OnGetLiveRoomInfo;
        SC_WebSocket.OnStartGame -= OnStartGame;
        SC_WebSocket.OnCancelMatching -= OnCancelMatching;
    }

    public void Awake()
    {
        unityObjects = SC_GlobalManager.Instance.UnityObjects;
        if(unityObjects.ContainsKey("Btn_Lobby_Disconnect"))
            unityObjects["Btn_Lobby_Disconnect"].SetActive(false);
        
        _properties = new Dictionary<string, object>(){{"Password","Shenkar"}};
    }
    #endregion

    #region Controller
    public void Btn_Lobby_Logout()
    {
        Debug.Log("Btn_Lobby_Logout");
        unityObjects["Screen_Lobby"].SetActive(false);
        unityObjects["Screen_Login"].SetActive(true);
        unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = string.Empty;
        unityObjects["UserId"].GetComponent<TextMeshProUGUI>().text = string.Empty;
        unityObjects["InputField_Login_Email"].GetComponent<TMP_InputField>().text = string.Empty;
        unityObjects["InputField_Login_Password"].GetComponent<TMP_InputField>().text = string.Empty;
    }
    public void Btn_Lobby_Play()
    {
        Debug.Log("Btn_Lobby_Play");
        UpdateStatus("Searching for an available rooms...");
        SC_WebSocket.Instance.GetRoomsInRange(minUsersCound, maxUsersCound);
    }
    public void Btn_SearchingOpponent_Cancel()
    {
        Debug.Log("Btn_SearchingOpponent_Cancel");
        unityObjects["Btn_SearchingOpponent_Cancel"].GetComponent<Button>().interactable = false;
        SC_WebSocket.Instance.CancelMatching();
    }
    public void Btn_SearchingOpponent_SendMessage()
    {
        Debug.Log("Btn_SearchingOpponent_Cancel");
        string _message = unityObjects["InputField_SearchingOpponent_Message"].GetComponent<TMP_InputField>().text;
        Dictionary<string, object> _data = new Dictionary<string, object>();
        _data.Add("Msg", _message);
        SC_WebSocket.Instance.SendServerMessage(_data);
    }

    #endregion

    #region Logic
    private void DoRoomSearchLogic()
    {
        if (_roomsIndex < _roomsData.Count)
        {
            UpdateStatus("Bring room info (" + _roomsData[_roomsIndex].RoomId + ")");
            SC_WebSocket.Instance.GetLiveRoomInfo(_roomsData[_roomsIndex].RoomId.ToString());
        }
        else
        {
            UpdateStatus("Creating Room...");
            SC_WebSocket.Instance.CreateTurnRoom("TicTacToe Room", UserManager.CurUser.UserId,
                maxUsersCound, _properties, turnTime);
        }
    }
    private void UpdateStatus(string _Str)
    {
        unityObjects["Txt_Status"].GetComponent<TextMeshProUGUI>().text = _Str;
    }
    #endregion

    #region Events

    private void OnConnect(Dictionary<string, object> _Data)
    {
        Debug.Log("OnConnect ");
        if (_Data.ContainsKey("IsOpen"))
        {
            bool _isOpen = bool.Parse(_Data["IsOpen"].ToString());
            if (_isOpen)
            {
                UpdateStatus("Connected.");
                unityObjects["Btn_Lobby_Connect"].SetActive(false);
                unityObjects["Btn_Lobby_Logout"].SetActive(false);
                unityObjects["Btn_Lobby_Disconnect"].SetActive(true);
                unityObjects["Btn_Lobby_Play"].SetActive(true);
            }
            else UpdateStatus("Failed to Connect.");
        }
        else UpdateStatus("Failed to Connect.");
    }
    private void OnDisconnect(Dictionary<string, object> _Data)
    {
        Debug.Log("OnDisconnect ");
        UpdateStatus("Disconnected");
        unityObjects["RoomId"].GetComponent<TextMeshProUGUI>().text = string.Empty;
        unityObjects["Btn_Lobby_Connect"].SetActive(true);
        unityObjects["Btn_Lobby_Logout"].SetActive(true);
        unityObjects["Btn_Lobby_Disconnect"].SetActive(false);
        unityObjects["Btn_Lobby_Play"].SetActive(false);
        unityObjects["Screen_Lobby"].SetActive(true);
        unityObjects["Screen_SearchingOpponent"].SetActive(false);
    }
    private void OnGetRoomsInRange(List<RoomData> roomsData)
    {
        _roomsIndex = 0;
        _roomsData = roomsData;

        UpdateStatus("Parsing Rooms...");
        if (_roomsData.Count > 0)
            SC_WebSocket.Instance.GetLiveRoomInfo(_roomsData[_roomsIndex].RoomId.ToString());
        else SC_WebSocket.Instance.CreateTurnRoom("TicTacToe Room", UserManager.CurUser.UserId, 
            maxUsersCound, _properties, turnTime);
    }
    private void OnCreateTurnRoom(bool isSuccess, string roomId)
    {
        if(isSuccess)
        {
            _roomId = roomId;
            SC_WebSocket.Instance.JoinRoom(_roomId);
            SC_WebSocket.Instance.SubscribeRoom(_roomId);
            UpdateStatus("Room have been created, RoomId: " + roomId);
        }
        else
        {
            UpdateStatus("Failed to create a room.");
        }
    }
    private void OnJoinRoom(bool isSuccess, string roomId)
    {
        if (isSuccess)
        {
            Debug.Log("Joined Room " + roomId);
            UpdateStatus("Joined Room: " + roomId);
            unityObjects["RoomId"].GetComponent<TextMeshProUGUI>().text = "RoomId: " + roomId;
            unityObjects["Btn_Lobby_Connect"].SetActive(false);
            unityObjects["Btn_Lobby_Logout"].SetActive(false);
            unityObjects["Btn_Lobby_Play"].SetActive(false);

            unityObjects["Screen_Lobby"].SetActive(false);
            unityObjects["Screen_SearchingOpponent"].SetActive(true);

        }
        else
        {
            Debug.Log("Cant Joined Room " + roomId);
            UpdateStatus("Cant Joined Room: " + roomId);
        }
    }
    private void OnUserJoinRoom(RoomData roomData, string userId)
    {
        UpdateStatus("User Joined Room " + userId);
        if (roomData.RoomOwner == UserManager.CurUser.UserId && UserManager.CurUser.UserId != userId)
        {
            Debug.Log("OnUserJoinRoom Room is full, starting game..");
            UpdateStatus("User Joined Room " + userId + ", Starting Game...");
            SC_WebSocket.Instance.StartGame();
        }
        else Debug.Log("OnUserJoinRoom Room isn't full, waiting for more players...");
    }
    private void OnSubscribeRoom(bool isSuccess, string roomId)
    {
        if (isSuccess)
            Debug.Log("Subscribed to Room " + roomId);
        else Debug.Log("Cant Subscribe to Room " + roomId);
    }
    private void OnGetLiveRoomInfo(RoomData roomData, List<string> users, Dictionary<string, object> properties)
    {
        if (roomData != null && properties.ContainsKey("Password") &&
            properties["Password"].ToString() == properties["Password"].ToString())
        {
            UpdateStatus("Received Room Info, joining room: " + roomData.RoomId);
            _curRoomData = roomData;
            SC_WebSocket.Instance.JoinRoom(_curRoomData.RoomId.ToString());
            SC_WebSocket.Instance.SubscribeRoom(_curRoomData.RoomId.ToString());
        }
        else
        {
            _roomsIndex++;
            DoRoomSearchLogic();
        }
    }
    private void OnCancelMatching(bool isSuccess,string errorMessage)
    {
        Debug.Log("OnCancelMatching ");
        if (isSuccess)
        {
            unityObjects["Btn_SearchingOpponent_Cancel"].GetComponent<Button>().interactable = true;
            unityObjects["Btn_Lobby_Play"].SetActive(true);
            unityObjects["Screen_Lobby"].SetActive(true);
            unityObjects["Screen_SearchingOpponent"].SetActive(false);
        }
        else Debug.Log("isSuccess: " + isSuccess + ", Message: " + errorMessage);
       
    }
    private void OnStartGame(string sender, string curRoomId, string nextTurn,int turnTime, List<string> turnsList)
    {
        Debug.Log("OnStartGame " + turnTime);
        unityObjects["Screen_SearchingOpponent"].SetActive(false);
        unityObjects["Menu"].SetActive(false);
        unityObjects["Game"].SetActive(true);

        SC_GameLogic.Instance.StartGame(curRoomId,turnTime, nextTurn, UserManager.CurUser.UserId, turnsList);
    }
    internal void Btn_Lobby_Connect()
    {
        SC_WebSocket.Instance.OpenConnection();
        UpdateStatus("Open Connection...");
    }
    internal void Btn_Lobby_Disconnect()
    {
        if (SC_WebSocket.Instance.IsOpen())
            SC_WebSocket.Instance.CloseConnection();

        unityObjects["Btn_Lobby_Connect"].SetActive(true);
        unityObjects["Btn_Lobby_Logout"].SetActive(true);
        unityObjects["Btn_Lobby_Disconnect"].SetActive(false);
        unityObjects["Btn_Lobby_Play"].SetActive(false);
    }

    #endregion
}
