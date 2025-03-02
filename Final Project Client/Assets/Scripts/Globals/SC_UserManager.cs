using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager
{
    private static User curUser;
    public static User CurUser { get => curUser; set => curUser = value; }

}
