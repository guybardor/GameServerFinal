using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_LoginLogic : MonoBehaviour
{
    private Dictionary<string, GameObject> unityObjects;
   
    #region Singleton
    static SC_LoginLogic instance;
    public static SC_LoginLogic Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_LoginLogic").GetComponent<SC_LoginLogic>();

            return instance;
        }
    }
    #endregion

    #region Monobehaviour
    private void OnEnable()
    {
        SC_LoginServerApi.OnGeneralResponse += OnGeneralResponse;
    }

    private void OnDisable()
    {
        SC_LoginServerApi.OnGeneralResponse -= OnGeneralResponse;
    }

    public void Awake()
    {
        unityObjects = SC_GlobalManager.Instance.UnityObjects;
    }

    private void Start()
    {
        unityObjects["Screen_Register"].SetActive(false);
        unityObjects["Screen_Lobby"].SetActive(false);
        if (unityObjects.ContainsKey("Screen_SearchingOpponent"))
            unityObjects["Screen_SearchingOpponent"].SetActive(false);
        if (unityObjects.ContainsKey("Game"))
            unityObjects["Game"].SetActive(false);
        if (unityObjects.ContainsKey("Btn_Lobby_Play"))
            unityObjects["Btn_Lobby_Play"].SetActive(false);
        if (unityObjects.ContainsKey("Btn_Lobby_Connect"))
            unityObjects["Btn_Lobby_Connect"].SetActive(true);
        if (unityObjects.ContainsKey("Btn_Lobby_Logout"))
            unityObjects["Btn_Lobby_Logout"].SetActive(true);
        if (unityObjects.ContainsKey("Btn_Lobby_Disconnect"))
            unityObjects["Btn_Lobby_Disconnect"].SetActive(false);

        unityObjects["InputField_Login_Email"].GetComponent<TMP_InputField>().text = "a@gmail.com";
        unityObjects["InputField_Login_Password"].GetComponent<TMP_InputField>().text = "12345";
    }

    #endregion


    #region Controller
    public void Btn_Login_Login()
    {
        Debug.Log("Btn_Login_Login");
        unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = string.Empty;

        string email = unityObjects["InputField_Login_Email"].GetComponent<TMP_InputField>().text;
        string _password = unityObjects["InputField_Login_Password"].GetComponent<TMP_InputField>().text;
        if (email.Length > 0 && _password.Length > 0)
        {
            if (IsValidEmail(email))
            {
                SC_LoginServerApi.Instance.Login(email, _password);
                unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = "Sent...";
            }
            else unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = "Broken email...";
        }
        else unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = "Missing Email/Password";
    }
    public void Btn_Login_Register()
    {
        Debug.Log("Btn_Login_Register");
        unityObjects["Screen_Register"].SetActive(true);
        unityObjects["Screen_Login"].SetActive(false);
        unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = string.Empty;
        unityObjects["InputField_Register_Email"].GetComponent<TMP_InputField>().text = string.Empty;
        unityObjects["InputField_Register_Password"].GetComponent<TMP_InputField>().text = string.Empty;
        unityObjects["InputField_Register_Password_Repeat"].GetComponent<TMP_InputField>().text = string.Empty;
    }
    public void Btn_Register_Register()
    {
        Debug.Log("Btn_Register_Register");
        unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = string.Empty;

        string _email = unityObjects["InputField_Register_Email"].GetComponent<TMP_InputField>().text;
        string _password = unityObjects["InputField_Register_Password"].GetComponent<TMP_InputField>().text;
        string _passwordRepeat = unityObjects["InputField_Register_Password_Repeat"].GetComponent<TMP_InputField>().text;
        if (_password == _passwordRepeat && _password.Length > 0)
        {
            if(IsValidEmail(_email))
            {
                Dictionary<string, object> _data = new Dictionary<string, object>();
                _data.Add("Email", _email);
                _data.Add("Password", _password);
                SC_LoginServerApi.Instance.Register(_data);
                unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = "Sent...";
            }
            else unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = "Broken email...";
        }
        else unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = "Password dont match..."; 
    }
    public void Btn_Register_Back()
    {
        Debug.Log("Btn_Register_Back");
        unityObjects["Screen_Register"].SetActive(false);
        unityObjects["Screen_Login"].SetActive(true);
        unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = string.Empty;
        unityObjects["InputField_Login_Email"].GetComponent<TMP_InputField>().text = string.Empty;
        unityObjects["InputField_Login_Password"].GetComponent<TMP_InputField>().text = string.Empty;
    }
    public void Btn_Lobby_AddXp()
    {
        string _value = unityObjects["InputField_Xp"].GetComponent<TMP_InputField>().text;
        if (_value.Length > 0)
        {
            Dictionary<string, object> _data = new Dictionary<string, object>();
            _data.Add("UserId", UserManager.CurUser.UserId);
            _data.Add("XpAmount", _value);
            SC_LoginServerApi.Instance.AddXp(_data);
        }
        else unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = "Xp amount value is empty";

    }
    public void Btn_Lobby_AddCurrency()
    {
        string _value = unityObjects["InputField_Currency"].GetComponent<TMP_InputField>().text;
        if (_value.Length > 0)
        {
            Dictionary<string, object> _data = new Dictionary<string, object>();
            _data.Add("UserId", UserManager.CurUser.UserId);
            _data.Add("CurrencyAmount", _value);
            SC_LoginServerApi.Instance.AddCurrency(_data);
        }
        else unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = "Currency amount value is empty";
    }
    #endregion

    private void OnGeneralResponse(Dictionary<string, object> _Data)
    {
        if (_Data.ContainsKey("Response"))
        {
            switch (_Data["Response"].ToString())
            {
                #region Register
                case "Register": RegisterResponse(_Data); break;
                #endregion
                #region Login
                case "Login":LoginResponse(_Data); break;
                #endregion
                #region PutXp
                case "PutXp": PutXpResponse(_Data); break;
                #endregion
                #region PutCurrency
                case "PutCurrency": PutCurrencyResponse(_Data); break;
                #endregion



            }
        }
    }
    #region Logic 

    bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
        {
            return false; // suggested by @TK-421
        }
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }
    private void RegisterResponse(Dictionary<string, object> _Data)
    {
        Debug.Log("RegisterResponse " + _Data.Count);
        unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = string.Empty;
        if (_Data.ContainsKey("ErrorCode"))
        {
            unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = _Data["ErrorCode"].ToString();
        }
        else if (_Data.ContainsKey("IsCreated"))
        {
            bool _isCreated = bool.Parse(_Data["IsCreated"].ToString());
            if(_isCreated)
            {
                unityObjects["Screen_Register"].SetActive(false);
                unityObjects["Screen_Login"].SetActive(true);
                unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = "User was created!";
            }
            else unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = "Failed to create a user";
        }
    }
    private void LoginResponse(Dictionary<string, object> _Data)
    {
        Debug.Log("RegisterResponse " + _Data.Count);
        unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = string.Empty;
        if (_Data.ContainsKey("ErrorCode"))
        {
            unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = _Data["ErrorCode"].ToString();
        }
        else if (_Data.ContainsKey("IsLoggedIn") && _Data.ContainsKey("UserId"))
        {
            bool _isLoggedIn = bool.Parse(_Data["IsLoggedIn"].ToString());
            if (_isLoggedIn)
            {
                string _email = unityObjects["InputField_Login_Email"].GetComponent<TMP_InputField>().text;
                string _userId = _Data["UserId"].ToString();
                UserManager.CurUser = new User(_email, _userId);
                Debug.Log("userId " + _userId);
                unityObjects["UserId"].GetComponent<TextMeshProUGUI>().text = _userId;
                unityObjects["Screen_Login"].SetActive(false);
                unityObjects["Screen_Lobby"].SetActive(true);
                unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = "Logged in";
                if (_Data.ContainsKey("GameServerUrl") && SC_WebSocket.Instance != null)
                {
                    string _gameServerUrl = _Data["GameServerUrl"].ToString();
                    Debug.Log("GameServer: " + _gameServerUrl);
                    SC_WebSocket.Instance.SetGameServerInfo(_gameServerUrl, _userId);
                }
            }
            else unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = "Failed to log in";
        }
    }

    private void PutXpResponse(Dictionary<string, object> data)
    {
        unityObjects["InputField_Box"].GetComponent<TMP_InputField>().text = string.Empty;
        foreach (string s in data.Keys)
            unityObjects["InputField_Box"].GetComponent<TMP_InputField>().text += s + " : " + data[s] + System.Environment.NewLine;
    }
    private void PutCurrencyResponse(Dictionary<string, object> _Data)
    {
        unityObjects["InputField_Box"].GetComponent<TMP_InputField>().text = string.Empty;
        foreach (string s in _Data.Keys)
            unityObjects["InputField_Box"].GetComponent<TMP_InputField>().text += s + " : " + _Data[s] + System.Environment.NewLine;
    }

    internal void Btn_Lobby_Logout()
    {
        unityObjects["Screen_Lobby"].SetActive(false);
        unityObjects["Screen_Login"].SetActive(true);
        unityObjects["Txt_Error"].GetComponent<TextMeshProUGUI>().text = string.Empty;
        unityObjects["UserId"].GetComponent<TextMeshProUGUI>().text = string.Empty;
        unityObjects["InputField_Login_Email"].GetComponent<TMP_InputField>().text = string.Empty;
        unityObjects["InputField_Login_Password"].GetComponent<TMP_InputField>().text = string.Empty;
    }

    #endregion
}
