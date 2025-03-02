using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GameController : MonoBehaviour
{
    public void Btn_GameOver_Restart()
    {
        SC_GameLogic.Instance.Btn_GameOver_Restart();
    }
    public void Btn_SendChat()
    {
        SC_GameLogic.Instance.Btn_SendChat();
    }
    public void Btn_ClearChat()
    {
        SC_GameLogic.Instance.Btn_ClearChat();
    }
}
