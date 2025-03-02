using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SC_LoginServerApi : MonoBehaviour
{
    public enum RequestMethod
    {
        Get, Post, Put, Delete
    };

    public enum Environment
    {
        Local,Development
    };

    public delegate void GeneralResponseHandler(Dictionary<string, object> _Data);
    public static event GeneralResponseHandler OnGeneralResponse;

    public Environment curEnvironment = Environment.Local;
    public string uri = "http://localhost:8080";
    public string uriDev = "http://52.51.66.123:8080/gamelobby";
    private string currentUri = "http://localhost:8080";

    #region Singleton
    static SC_LoginServerApi instance;
    public static SC_LoginServerApi Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_LoginServerApi").GetComponent<SC_LoginServerApi>();

            return instance;
        }
    }
    #endregion

    private void Start()
    {
        switch(curEnvironment)
        {
            case Environment.Development: currentUri = uriDev; break;
            case Environment.Local: currentUri = uri; break;
        }
    }

    private IEnumerator ServerMethodRequestIEnumerator(RequestMethod _Method, string _Uri, string _PostData)
    {
        using (UnityWebRequest webRequest = GetUnityWebRequest(_Method, _Uri, _PostData))
        {
            webRequest.SetRequestHeader("content-type", "application/json");
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
    private UnityWebRequest GetUnityWebRequest(RequestMethod _Method, string _Uri, string _PostData)
    {
        switch (_Method)
        {
            case RequestMethod.Delete:
                UnityWebRequest _webDeleteRequest = UnityWebRequest.Delete(_Uri);
                _webDeleteRequest.downloadHandler = new DownloadHandlerBuffer();
                return _webDeleteRequest;
            case RequestMethod.Get: return UnityWebRequest.Get(_Uri);
            case RequestMethod.Post:
                UnityWebRequest _webPostRequest = UnityWebRequest.Post(_Uri, _PostData);
                _webPostRequest.uploadHandler.contentType = "application/json";
                _webPostRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(_PostData));
                return _webPostRequest;
            case RequestMethod.Put: return UnityWebRequest.Put(_Uri, _PostData);
        }
        return new UnityWebRequest(_Uri);
    }

    public void Register(Dictionary<string, object> _Data)
    {
        Debug.Log("<color=blue>Login: </color>" + uri + "/Register");
        StartCoroutine(ServerMethodRequestIEnumerator(RequestMethod.Post, uri + "/Register", MiniJSON.Json.Serialize(_Data)));
    }
    public void Login(string _Email,string _Password)
    {
        Debug.Log("<color=blue>GenerateRandomName: </color>" + uri + "/Login/"+ _Email + "&" + _Password);
        StartCoroutine(ServerMethodRequestIEnumerator(RequestMethod.Get, uri + "/Login/" + _Email + "&" + _Password, string.Empty));
    }

    public void AddXp(Dictionary<string, object> _Data)
    {
        Debug.Log("<color=blue>AddXp: </color>" + uri + "/PutXp");
        StartCoroutine(ServerMethodRequestIEnumerator(RequestMethod.Put, uri + "/PutXp", MiniJSON.Json.Serialize(_Data)));
    }
    public void AddCurrency(Dictionary<string, object> _Data)
    {
        Debug.Log("<color=blue>PutCurrency: </color>" + uri + "/PutCurrency");
        StartCoroutine(ServerMethodRequestIEnumerator(RequestMethod.Put, uri + "/PutCurrency", MiniJSON.Json.Serialize(_Data)));
    }
}

