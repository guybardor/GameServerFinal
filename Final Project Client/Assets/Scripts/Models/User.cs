using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    private string userId;
    public string UserId { get => userId; }

    private string email;
    public string Email { get => email; }

    public User(string _Email,string _UserId)
    {
        email = _Email;
        userId = _UserId;
    }

}
