using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GameData : MonoBehaviour
{
    private Dictionary<string, Sprite> unitySprites;

    #region Singleton

    private static SC_GameData instance;
    public static SC_GameData Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_GameData").GetComponent<SC_GameData>();

            return instance;
        }
    }

    #endregion
    void Awake()
    {
        Init();
    }

    private void Init()
    {
        unitySprites = new Dictionary<string, Sprite>();
        List<string> _spritesToLoad = new List<string>();
        _spritesToLoad.Add("Sprite_X");
        _spritesToLoad.Add("Sprite_O");

        foreach(string s in _spritesToLoad)
        {
            Texture2D _texture2d = Resources.Load("Sprites/" + s) as Texture2D;
            Sprite _newSprite = Sprite.Create(_texture2d, new Rect(0, 0, _texture2d.width, _texture2d.height), new Vector2(0.5f,0.5f));
            unitySprites.Add(s, _newSprite);
        }

      //  Debug.Log(unitySprites.Count);
    }

    public Sprite GetSprite(string _SpritName)
    {
        //Debug.Log(_SpritName);
        if (unitySprites.ContainsKey(_SpritName))
            return unitySprites[_SpritName];
        else return null;
    }
}
