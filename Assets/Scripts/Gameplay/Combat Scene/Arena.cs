using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arena : MonoBehaviour
{
    public GameObject mCellPrefab;
    [HideInInspector]
    public List<Cell> mAllCells = new List<Cell>();

    public void Create()
    {
        GameObject newCell = Instantiate(mCellPrefab, transform);
        RectTransform rectTransform = newCell.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 0);

        Cell cell = newCell.GetComponent<Cell>();
        cell.Setup(new Vector2Int(0, 0), this);
        //cell.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        mAllCells.Add(cell);

    }
}
