using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Slot : MonoBehaviour
{
    public delegate void ClickHandler(int _Index);
    public static event ClickHandler OnClick;

    private SpriteRenderer spriteRen;

    public int slotIdx = 0;
    void Start()
    {
        // Debug.Log(name);
        spriteRen = GetComponent<SpriteRenderer>();
    }

    void OnMouseUp()
    {
        // Debug.Log("OnMouseUp " + name);
        if (OnClick != null)
            OnClick(slotIdx);
    }

    public void SetSprite(Sprite _NewSprite)
    {
        if (spriteRen != null)
            spriteRen.sprite = _NewSprite;
        else if (_NewSprite == null)
            Debug.LogError("NewSprite is Null!!");
        else Debug.LogError("spriteRen is Null!!");
    }
}
