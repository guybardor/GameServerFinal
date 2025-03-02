using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SC_EmployeeServerApi : MonoBehaviour
{
    public enum RequestMethod
    {
        Get,Post,Put,Delete
    };

    public delegate void GeneralResponseHandler(Dictionary<string, object> _Data);
    public static event GeneralResponseHandler OnGeneralResponse;
    public string uri = "https://localhost:7225/api";

    #region Singleton
    static SC_EmployeeServerApi instance;
    public static SC_EmployeeServerApi Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_EmployeeServerApi").GetComponent<SC_EmployeeServerApi>();

            return instance;
        }
    }
    #endregion

    private IEnumerator ServerMethodRequestIEnumerator(RequestMethod _Method, string _Uri, string _PostData)
    {
        using (UnityWebRequest webRequest = GetUnityWebRequest(_Method,_Uri, _PostData))
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

    public void GetEmployee(string _Id)
    {
        Debug.Log("<color=blue>GenerateRandomName: </color>" + uri + "/GetEmployee/" + _Id);
        StartCoroutine(ServerMethodRequestIEnumerator(RequestMethod.Get, uri + "/GetEmployees/" + _Id, string.Empty));
    }
    public void PostEmployee(Dictionary<string, object> _Data)
    {
        Debug.Log("<color=blue>Login: </color>" + uri + "/PostEmployee");
        StartCoroutine(ServerMethodRequestIEnumerator(RequestMethod.Post, uri + "/PostEmployee", MiniJSON.Json.Serialize(_Data)));
    }
    public void PutEmployee(Dictionary<string, object> _Data)
    {
        Debug.Log("<color=blue>Login: </color>" + uri + "/PutEmployee");
        StartCoroutine(ServerMethodRequestIEnumerator(RequestMethod.Put, uri + "/PutEmployee", MiniJSON.Json.Serialize(_Data)));
    }
    public void DeleteEmployee(string _Id)
    {
        Debug.Log("<color=blue>Login: </color>" + uri + "/DeleteEmployee/" + _Id);
        StartCoroutine( ServerMethodRequestIEnumerator(RequestMethod.Delete,uri + "/DeleteEmployee/" + _Id,string.Empty));
    }
}