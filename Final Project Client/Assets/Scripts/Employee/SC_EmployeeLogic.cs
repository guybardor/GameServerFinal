using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class SC_EmployeeLogic : MonoBehaviour
{
    public Dictionary<string, GameObject> unityObjects;

    #region Singleton
    static SC_EmployeeLogic instance;
    public static SC_EmployeeLogic Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_EmployeeLogic").GetComponent<SC_EmployeeLogic>();

            return instance;
        }
    }

    #endregion
    #region Monobehaviour
    private void OnEnable()
    {
        SC_EmployeeServerApi.OnGeneralResponse += OnGeneralResponse;
    }

    private void OnDisable()
    {
        SC_EmployeeServerApi.OnGeneralResponse -= OnGeneralResponse;
    }

    private void Start()
    {
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] _battleObjects = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach (GameObject g in _battleObjects)
            unityObjects.Add(g.name, g);
    }

    // Start is called before the first frame update
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //    Btn_EmployeesPost("51235435", "Moshe");
        //if (Input.GetKeyDown(KeyCode.S))
        //    Btn_EmployeesGet("51235435");
        //if (Input.GetKeyDown(KeyCode.D))
        //    Btn_EmployeesDelete("51235435");
        //if (Input.GetKeyDown(KeyCode.P))
        //    Btn_EmployeesPut("51235435", "Rand");
    }
    #endregion

    #region Controller

    //Delete User
    public void Btn_EmployeesDelete(string _Id)
    {
        SC_EmployeeServerApi.Instance.DeleteEmployee(_Id);
    }

    //Get All User
    public void Btn_EmployeesGet(string _Id)
    {
        SC_EmployeeServerApi.Instance.GetEmployee(_Id);
    }

    //Create User
    public void Btn_EmployeesPost(string _Id,string _Name)
    {
        Dictionary<string, object> _data = new Dictionary<string, object>() { 
            { "Id", _Id },
            { "Name", _Name }
        };
        SC_EmployeeServerApi.Instance.PostEmployee(_data);
    }

    //Update user
    public void Btn_EmployeesPut(string _Id, string _Name)
    {
        Dictionary<string, object> _data = new Dictionary<string, object>() {
            { "Id", _Id },
            { "Name", _Name }
        };
        SC_EmployeeServerApi.Instance.PutEmployee(_data);
    }

    #endregion

    #region Server Callbacks

    private void OnGeneralResponse(Dictionary<string, object> _Data)
    {
        if (_Data.ContainsKey("Message"))
        {
            unityObjects["InputField_Box"].GetComponent<TMP_InputField>().text = string.Empty;
            foreach (string s in _Data.Keys)
                unityObjects["InputField_Box"].GetComponent<TMP_InputField>().text += s + " : " + _Data[s] + System.Environment.NewLine;
        }
    }
    #endregion
}
