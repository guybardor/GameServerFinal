using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_PingController : MonoBehaviour
{
    public void Btn_PingGet()
    {
        SC_PingLogic.Instance.Btn_PingGet();
    }
    public void Btn_PingPost()
    {
        SC_PingLogic.Instance.Btn_PingPost();
    }
}
