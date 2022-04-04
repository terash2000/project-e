using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviourSingletonPersistent<CursorController>
{
    public Texture2D DefaultCursor;

    // Start is called before the first frame update
    void Start()
    {
        Default();
    }

    public void Default()
    {
        Cursor.SetCursor(DefaultCursor, Vector2.zero, CursorMode.Auto);
    }
}
