using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_GameLogic : MonoBehaviour
{
    public enum PlayerState
    {
        MyTurn,NotMyTurn
    };

    #region Singleton

    private static SC_GameLogic instance;
    public static SC_GameLogic Instance
    {
        get 
        {
            if (instance == null)
                instance = GameObject.Find("SC_GameLogic").GetComponent<SC_GameLogic>();

            return instance;
        }
    }

    #endregion

    private bool isMatchOver = false;
    private bool isTimer = false;
    private Dictionary<string, GameObject> unityObjects;
    private SC_Board curBoard;
    private SC_Board.SlotState curState;
    private PlayerState curPlayerState;

    private string roomId;
    private int maxTurnTime;
    private System.DateTime startTime;
    private float moveStartTime;
    private string myPlayerId;
    private string curPlayerId;
    private List<string> players;

    public Sprite O;
    public Sprite X;

    #region MonoBehaviour
    private void OnEnable()
    {
        SC_Slot.OnClick += OnClick;
        SC_WebSocket.OnBroadcastMove += OnBroadcastMove;
        SC_WebSocket.OnGameStopped += OnGameStopped;
        SC_WebSocket.OnLeaveRoom += OnLeaveRoom;
        SC_WebSocket.OnSendChat += OnSendChat;

        //SC_WebSocket.OnPassTurn += OnPassTurn;

    }

    private void OnDisable()
    {
        SC_Slot.OnClick -= OnClick;
        SC_WebSocket.OnBroadcastMove -= OnBroadcastMove;
        SC_WebSocket.OnGameStopped -= OnGameStopped;
        SC_WebSocket.OnLeaveRoom -= OnLeaveRoom;
        SC_WebSocket.OnSendChat -= OnSendChat;

        // SC_WebSocket.OnPassTurn -= OnPassTurn;
    }

    void Awake()
    {
        InitAwake();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            SC_WebSocket.Instance.SendChat(roomId,"Bla bla " + Time.time);

        if (isTimer)
        {
            int _leftover = maxTurnTime - (int)(Time.time - moveStartTime);
            if (_leftover < 0)
                _leftover = 0;

            if(_leftover < 10)
                unityObjects["Timer"].GetComponent<TextMeshProUGUI>().text = "0"+_leftover.ToString();
            else unityObjects["Timer"].GetComponent<TextMeshProUGUI>().text = _leftover.ToString();
        }
    }

    #endregion

    #region Logic

    private void InitAwake()
    {
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] _objs = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach (GameObject g in _objs)
            unityObjects.Add(g.name, g);
    }
    private void PassTurn()
    {
        curPlayerState = PlayerState.NotMyTurn;
        if (curState == SC_Board.SlotState.O)
            curState = SC_Board.SlotState.X;
        else if (curState == SC_Board.SlotState.X)
            curState = SC_Board.SlotState.O;
        else Debug.LogError("curState is Not O or X !");

        unityObjects["Img_CurrentTurn"].GetComponent<Image>().sprite = GetSprite("Sprite_" + curState.ToString());
    }
    private Sprite GetSprite(string _SpriteName)
    {
        return SC_GameData.Instance.GetSprite("Sprite_" + curState.ToString());
    }
    private IEnumerator PlayAI()
    {
        int _rand = UnityEngine.Random.Range(2,4);
        Debug.Log("Bot wait time " + _rand);
        yield return new WaitForSeconds(_rand);
        int _index = curBoard.GetEmptySlot();

        Debug.Log("Bot is Playing " + _index);
        Placement(_index,false);
    }

    private void Placement(int _Index,bool _IsSimulation)
    {
        SC_Slot _cuSlot = unityObjects["Btn_Slot" + _Index].GetComponent<SC_Slot>();
        SC_Board.SlotState _currentState = curState;
        if (_cuSlot != null)
        {
            curPlayerState = PlayerState.NotMyTurn;
            curBoard.SetSlotState(_Index, curState);
            _cuSlot.SetSprite(GetSprite("Sprite_" + curState.ToString()));
            SC_Board.WinnerState _curWinState = curBoard.IsMatchOver();
            if (_curWinState == SC_Board.WinnerState.Tie || _curWinState == SC_Board.WinnerState.Winner)
            {
                Debug.Log("Match Over, WinCondition: " + _curWinState + ". CurrentState " + curState);
                isMatchOver = true;
                unityObjects["PopUp_GameOver"].SetActive(true);
                if (_curWinState == SC_Board.WinnerState.Tie)
                {
                    unityObjects["Txt_GameOver_Description"].GetComponent<Text>().text = "The Game is Tied.";
                    unityObjects["Img_GameOver_Winner"].GetComponent<Image>().sprite = null;
                    if (_IsSimulation == false)
                        SC_WebSocket.Instance.StopGame("Tie");
                }
                else if (_curWinState == SC_Board.WinnerState.Winner)
                {
                    unityObjects["Txt_GameOver_Description"].GetComponent<Text>().text = "The Winner is:";
                    unityObjects["Img_GameOver_Winner"].GetComponent<Image>().sprite = GetSprite("Sprite_" + curState);
                    if(_IsSimulation == false)
                        SC_WebSocket.Instance.StopGame(curPlayerId);
                }
            }
            else PassTurn();
        }
    }

    public void StartGame(string _RoomId,int _MaxTurnTime, string _CurPlayerId,string _MyPlayerId, List<string> _Players)
    {
        try
        {
            roomId = _RoomId;
            maxTurnTime = _MaxTurnTime;
            startTime = DateTime.UtcNow;
            myPlayerId = _MyPlayerId;
            curPlayerId = _CurPlayerId;
            players = _Players;
            isTimer = true;
            moveStartTime = Time.time;

            isMatchOver = false;
            curBoard = new SC_Board();
            curState = SC_Board.SlotState.X;

            unityObjects["PopUp_GameOver"].SetActive(false);
            unityObjects["Img_CurrentTurn"].GetComponent<Image>().sprite = GetSprite("Sprite_" + curState.ToString());
           // unityObjects["PlayerId"].GetComponent<TextMeshProUGUI>().text = myPlayerId;

            for (int i = 0; i < curBoard.MaxSlots; i++)
                unityObjects["Btn_Slot" + i].GetComponent<SC_Slot>().SetSprite(null);

            if (_CurPlayerId == myPlayerId)
            {
                curPlayerState = PlayerState.MyTurn;
                unityObjects["Img_MySign"].GetComponent<Image>().sprite = X;
            }
            else
            {
                curPlayerState = PlayerState.NotMyTurn;
                unityObjects["Img_MySign"].GetComponent<Image>().sprite = O;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    #endregion

    #region Events
    private void OnClick(int _Index)
    {
        //Debug.Log("Btn_Slot" + _Index);
        if (isMatchOver == false && curPlayerState == PlayerState.MyTurn && curBoard.GetSlotState(_Index) == SC_Board.SlotState.Empty
            && unityObjects.ContainsKey("Btn_Slot" + _Index))
        {
            SC_WebSocket.Instance.SendMove(_Index.ToString());
            Placement(_Index,false);
        }
    }
    private void OnBroadcastMove(Move moveData)
    {
        try
        {
            if (moveData.MoveData == string.Empty)
                 PassTurn();
            else if (moveData.Sender != myPlayerId)
            {
                int _index = int.Parse(moveData.MoveData);
                Placement(_index, true);
            }
        }
        catch (Exception e) { Debug.LogError("OnBroadcastMove " + e.Message); }

        if (moveData.NextTurn == myPlayerId)
            curPlayerState = PlayerState.MyTurn;
        else curPlayerState = PlayerState.NotMyTurn;

        moveStartTime = Time.time;
    }
    private void OnPassTurn(Dictionary<string, object> _PassedData)
    {
        Debug.Log(_PassedData.Count);
        PassTurn();
        if (_PassedData["CP"].ToString() == myPlayerId)
            curPlayerState = PlayerState.MyTurn;
        else curPlayerState = PlayerState.NotMyTurn;

        if (_PassedData.ContainsKey("MC"))
            unityObjects["MoveCounter"].GetComponent<TextMeshProUGUI>().text = "Moves Counter: " + _PassedData["MC"].ToString();

        moveStartTime = Time.time;
    }
    private void OnGameStopped(string sender,string roomId)
    {
        Debug.Log("OnStopGame ");
        isMatchOver = true;
        isTimer = false;
        //if(_PassedData.ContainsKey("Winner") && _PassedData["Winner"].ToString() == "Tie")
        //{
        //    unityObjects["PopUp_GameOver"].SetActive(true);
        //    unityObjects["Txt_GameOver_Description"].GetComponent<Text>().text = "The Game is Tied.";
        //    unityObjects["Img_GameOver_Winner"].GetComponent<Image>().sprite = null;
        //}
        //if (_PassedData.ContainsKey("MC"))
        //    unityObjects["MoveCounter"].GetComponent<TextMeshProUGUI>().text = "Moves Counter: " + _PassedData["MC"].ToString();
    }
    private void OnLeaveRoom(bool isSuccess, LeaveRoomData roomData)
    {
        if (isSuccess)
        {
            unityObjects["Game"].SetActive(false);
            unityObjects["Menu"].SetActive(true);
            unityObjects["Screen_SearchingOpponent"].SetActive(false);
            unityObjects["Screen_Login"].SetActive(false);
            unityObjects["Screen_Lobby"].SetActive(true);
            unityObjects["Btn_Lobby_Play"].SetActive(true);
        }
        else Debug.Log("OnLeaveRoom " + isSuccess);
    }

    private void OnSendChat(string userId, string roomId, string Message)
    {
        Debug.Log(userId + " " + roomId + " " + Message);
        string msg = userId + ": " + Message + System.Environment.NewLine + unityObjects["InputField_Display"].GetComponent<TMP_InputField>().text;
        unityObjects["InputField_Display"].GetComponent<TMP_InputField>().text = msg;
    }

    #endregion

    #region Controller
    public void Btn_GameOver_Restart()
    {
        //SC_WebSocket.Instance.CloseConnection();
        SC_WebSocket.Instance.LeaveRoom(roomId);
    }

    internal void Btn_SendChat()
    {
        if (unityObjects.ContainsKey("InputField_Message"))
        {
            string msg = unityObjects["InputField_Message"].GetComponent<TMP_InputField>().text;
            if (msg != string.Empty)
            {
                unityObjects["InputField_Message"].GetComponent<TMP_InputField>().text = string.Empty;
                SC_WebSocket.Instance.SendChat(roomId, msg);
            }
        }
    }

    internal void Btn_ClearChat()
    {
        unityObjects["InputField_Display"].GetComponent<TMP_InputField>().text = string.Empty;
    }

    #endregion
}
