using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GlobalManager
{
    private Dictionary<string, GameObject> unityObjects;
    public Dictionary<string, GameObject> UnityObjects { get => unityObjects; }

    #region Singleton

    static SC_GlobalManager instance;
    public static SC_GlobalManager Instance
    {
        get
        {
            if (instance == null)
                instance = new SC_GlobalManager();

            return instance;
        }
    }

    #endregion

    public SC_GlobalManager()
    {
        instance = this;

        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] _objects = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach (GameObject g in _objects)
            unityObjects.Add(g.name, g);
    }
}
