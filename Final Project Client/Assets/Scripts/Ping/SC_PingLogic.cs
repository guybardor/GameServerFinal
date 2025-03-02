using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SC_PingLogic : MonoBehaviour
{
    public Dictionary<string, GameObject> unityObjects;

    #region Singleton
    static SC_PingLogic instance;
    public static SC_PingLogic Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_PingLogic").GetComponent<SC_PingLogic>();

            return instance;
        }
    }
    #endregion

    #region Monobehaviour
    private void OnEnable()
    {
        SC_PingServerApi.OnGeneralResponse += OnGeneralResponse;
    }

    private void OnDisable()
    {
        SC_PingServerApi.OnGeneralResponse -= OnGeneralResponse;
    }

    private void Start()
    {
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] _battleObjects = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach (GameObject g in _battleObjects)
            unityObjects.Add(g.name, g);
    }
    #endregion

    #region Controller
    public void Btn_PingGet()
    {
        Debug.Log("Btn_PingGet");
        SC_PingServerApi.Instance.PingCheck();
    }
    public void Btn_PingPost()
    {
        Debug.Log("Btn_PingPost");
        Dictionary<string, object> _data = new Dictionary<string, object>{{ "SendTime",DateTime.UtcNow}};
       SC_PingServerApi.Instance.PingCheck(_data);
    }
    #endregion

    #region Events

    private void OnGeneralResponse(Dictionary<string, object> _Data)
    {
        if (_Data.ContainsKey("Response"))
        {
            switch (_Data["Response"].ToString())
            {
                #region Ping
                case "Ping": Ping(_Data); break;
                #endregion
                #region PingPost
                case "PingCheck": PingPost(_Data); break;
                #endregion
            }
        }
    }

    #endregion

    #region Logic

    private void Ping(Dictionary<string, object> _Data)
    {
        Debug.Log("Ping " + _Data.Count);
        unityObjects["InputField_Box"].GetComponent<TMP_InputField>().text = string.Empty;
        foreach (string s in _Data.Keys)
            unityObjects["InputField_Box"].GetComponent<TMP_InputField>().text += s + " : " + _Data[s] + System.Environment.NewLine;
    }

    private void PingPost(Dictionary<string, object> _Data)
    {
        Debug.Log("PingPost " + _Data.Count);
        unityObjects["InputField_Box"].GetComponent<TMP_InputField>().text = string.Empty;
        foreach (string s in _Data.Keys)
            unityObjects["InputField_Box"].GetComponent<TMP_InputField>().text += s + " : " +_Data[s] + System.Environment.NewLine;
    }


    #endregion
}
