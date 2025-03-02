using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SC_EpmloyeeController : MonoBehaviour
{
    public void Btn_EmployeesGet()
    {
        string _id = GameObject.Find("InputField_EmployeeId").GetComponent <TMP_InputField>().text;
        SC_EmployeeLogic.Instance.Btn_EmployeesGet(_id);
    }
    public void Btn_EmployeesPut()
    {
        string _id = GameObject.Find("InputField_EmployeeId").GetComponent<TMP_InputField>().text;
        string _name = GameObject.Find("InputField_EmployeeName").GetComponent<TMP_InputField>().text;
        SC_EmployeeLogic.Instance.Btn_EmployeesPut(_id, _name);
    }
    public void Btn_EmployeesPost()
    {
        string _id = GameObject.Find("InputField_EmployeeId").GetComponent<TMP_InputField>().text;
        string _name = GameObject.Find("InputField_EmployeeName").GetComponent<TMP_InputField>().text;
        SC_EmployeeLogic.Instance.Btn_EmployeesPost(_id, _name);
    }
    public void Btn_EmployeesDelete()
    {
        string _id = GameObject.Find("InputField_EmployeeId").GetComponent<TMP_InputField>().text;
        SC_EmployeeLogic.Instance.Btn_EmployeesDelete(_id);
    }
}
