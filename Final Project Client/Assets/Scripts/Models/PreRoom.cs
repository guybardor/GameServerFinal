using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    private int _roomId;
    public int RoomId { get => _roomId; }

    private string _roomName;
    public string RoomName { get => _roomName; }

    private int _turnTime;
    public int TurnTime { get => _turnTime; }

    private string _roomOwner;
    public string RoomOwner { get => _roomOwner; }

    private int _maxUsersCount;
    public int MaxUsersCount { get => _maxUsersCount; }

    private int _joinedUsersCount;
    public int JoinedUsersCount { get => _joinedUsersCount; }


    public RoomData(int roomId, string roomName, int turnTime, string roomOwner, int maxUsersCount, int joinedUsersCount)
    {
        _roomId = roomId;
        _roomName = roomName;
        _turnTime = turnTime;
        _roomOwner = roomOwner;
        _maxUsersCount = maxUsersCount;
        _joinedUsersCount = joinedUsersCount;
    }

    public static RoomData Create(Dictionary<string, object> _Data)
    {
        int roomId = -1;
        int turnTime = -1;
        string roomName = string.Empty;
        string roomOwner = string.Empty;
        int maxUsersCount = -1;
        int joinedUsersCount = -1;

        if (_Data.ContainsKey("RoomId"))
            roomId = int.Parse(_Data["RoomId"].ToString());

        if (_Data.ContainsKey("Name"))
            roomName = _Data["Name"].ToString();

        if (_Data.ContainsKey("TurnTime"))
            turnTime = int.Parse(_Data["TurnTime"].ToString());

        if (_Data.ContainsKey("Owner"))
            roomOwner = _Data["Owner"].ToString();

        if (_Data.ContainsKey("MaxUsersCount"))
            maxUsersCount = int.Parse(_Data["MaxUsersCount"].ToString());

        if (_Data.ContainsKey("JoinedUsersCount"))
            joinedUsersCount = int.Parse(_Data["JoinedUsersCount"].ToString());

        if (roomId != -1 && roomName != string.Empty && roomOwner != string.Empty &&
            maxUsersCount != -1 && joinedUsersCount != -1)
            return new RoomData(roomId, roomName, turnTime, roomOwner, maxUsersCount, joinedUsersCount);
        else return null;
    }


}
