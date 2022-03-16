using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Image mOutlineImage;
    [HideInInspector]
    public Vector2Int mArenaPosition = Vector2Int.zero;
    [HideInInspector]
    public Arena mArena = null;
    [HideInInspector]
    public RectTransform mRectTransform = null;
    [HideInInspector]
    public Cell[] nearByCells;
    public void Setup(Vector2Int newArenaPosition, Arena newArena)
    {
        mArenaPosition = newArenaPosition;
        mArena = newArena;

        mRectTransform = GetComponent<RectTransform>();
    }

}
