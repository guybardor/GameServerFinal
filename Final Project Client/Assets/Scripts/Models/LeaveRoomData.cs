using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveRoomData 
{
    private string _roomId;
    public string RoomId { get => _roomId; }

    private string _owner;
    public string Owner { get => _owner; }

    private int _maxUsers;
    public int MaxUsers { get => _maxUsers; }

    private string _name;
    public string Name { get => _name; }


    public LeaveRoomData(string roomId, string owner, int maxUsers, string name)
    {
        _roomId = roomId;
        _owner = owner;
        _maxUsers = maxUsers;
        _name = name;
    }
}
