using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private string _sender;
    public string Sender { get => _sender; }

    private string _roomId;
    public string RoomId { get => _roomId; }

    private string _nextTurn;
    public string NextTurn { get => _nextTurn; }

    private string _moveData;
    public string MoveData { get => _moveData; }


    public Move(string sender, string roomId, string nextTurn, string moveData)
    {
        _sender = sender;
        _roomId = roomId;
        _nextTurn = nextTurn;
        _moveData = moveData;
    }
}
