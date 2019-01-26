using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (BoardManager.instance)
        {
            //Change Cursor to footsteps
            if (BoardManager.instance.IsPlayerMoving)
                Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
            else
                Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }
    }
}
