using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SC_PingServerApi : MonoBehaviour
{
    public delegate void GeneralResponseHandler(Dictionary<string, object> _Data);
    public static event GeneralResponseHandler OnGeneralResponse;
    public string uri = "https://localhost:7225/api";

    #region Singleton
    static SC_PingServerApi instance;
    public static SC_PingServerApi Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_PingServerApi").GetComponent<SC_PingServerApi>();

            return instance;
        }
    }
    #endregion

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            PingCheck();
        if (Input.GetKeyDown(KeyCode.S))
        {
            Dictionary<string, object> _data = new Dictionary<string, object>
            {
                { "Ping",DateTime.UtcNow}
            };
            PingCheck(_data);
        }
    }


    private IEnumerator ServerPostRequestIEnumerator(string _Uri, string _PostData)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(_Uri, _PostData))
        {
            webRequest.SetRequestHeader("content-type", "application/json");
            webRequest.uploadHandler.contentType = "application/json";
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(_PostData));
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.ConnectionError &&
                webRequest.result != UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("<color=green>Response: </color>" + webRequest.downloadHandler.text);
                Dictionary<string, object> _res = (Dictionary<string, object>)MiniJSON.Json.Deserialize(webRequest.downloadHandler.text);
                if (OnGeneralResponse != null)
                    OnGeneralResponse(_res);
            }
        }
    }
    private IEnumerator ServerGetRequestIEnumerator(string _Uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(_Uri))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.ConnectionError &&
                webRequest.result != UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("<color=green>Response: </color>" + webRequest.downloadHandler.text);
                Dictionary<string, object> _res = new Dictionary<string, object>() {
                    { "Response", "Ping" },
                    { "Result", webRequest.downloadHandler.text }
                };
                if (OnGeneralResponse != null)
                    OnGeneralResponse(_res);
            }
        }
    }
    private void GeneralPostRequest(string _Path, Dictionary<string, object> _Data)
    {
        StartCoroutine(ServerPostRequestIEnumerator(_Path, MiniJSON.Json.Serialize(_Data)));
    }
    public void PingCheck()
    {
        Debug.Log("<color=blue>GenerateRandomName: </color>" + uri + "/PingCheck");
        StartCoroutine(ServerGetRequestIEnumerator(uri + "/PingCheck"));
    }
    public void PingCheck(Dictionary<string, object> _Data)
    {
        Debug.Log("<color=blue>Login: </color>" + uri + "/PingCheck");
        GeneralPostRequest(uri + "/PingCheck", _Data);
    }


}
