using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Arena : MonoBehaviour
{
    public static Arena singleton;

    public Tile mTile;
    public Tile mOriginalTile;
    [HideInInspector]
    public GridLayout grid;
    [HideInInspector]
    public Tilemap tilemap;
    [HideInInspector]
    public GameObject hexBorder;
    [HideInInspector]
    public List<GameObject> hexBorderList;
    [HideInInspector]
    public GameObject redHexBorder;
    [HideInInspector]
    public CardController SelectedCard;


    void Awake()
    {
        singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponentInChildren<GridLayout>();
        tilemap = grid.GetComponentInChildren<Tilemap>();
        hexBorder = transform.Find("hexBorder").gameObject;
        BakeLineDebuger(hexBorder);
        hexBorder.gameObject.SetActive(false);
        redHexBorder = transform.Find("redHexBorder").gameObject;
        BakeLineDebuger(redHexBorder);
        SelectedCard = null;
    }

    public static void BakeLineDebuger(GameObject lineObj)
    {
        var lineRenderer = lineObj.GetComponent<LineRenderer>();
        var meshFilter = lineObj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh);
        meshFilter.sharedMesh = mesh;

        var meshRenderer = lineObj.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = lineRenderer.sharedMaterial;
        meshRenderer.sortingOrder = lineRenderer.sortingOrder;

        GameObject.Destroy(lineRenderer);
    }
    void OnMouseOver()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState != GameState.Running) return;

        Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mousePos = grid.WorldToCell(new Vector3(oriPos.x, oriPos.y, 0));
        Tile tile = (Tile)tilemap.GetTile(mousePos);

        if (MonsterManager.singleton.FindMonsterByTile(mousePos) != null)
        {
            // highlight monster
            hideBorder();
            redHexBorder.gameObject.SetActive(true);
            redHexBorder.transform.position = grid.CellToWorld(mousePos);
        }
        else if (tile != null && tile.Equals(mTile))
        {
            redHexBorder.gameObject.SetActive(false);
            showBorder(oriPos);
        }
        else
        {
            hideBorder();
            redHexBorder.gameObject.SetActive(false);
        }
    }

    public void showBorder(Vector3 targetPos)
    {
        List<Vector3Int> posList = getPosListTarget(SelectedCard.mTargetShape, SelectedCard.mRange, PlayerManager.singleton.Player.currentTile, targetPos);
        for (int i = 0; i < posList.Count; i++)
        {
            if (!tilemap.GetTile(posList[i]).Equals(mTile))
            {
                return;
            }
        }
        for (int i = 0; i < posList.Count; i++)
        {
            GameObject border;
            if (i >= hexBorderList.Count)
            {
                border = GameObject.Instantiate(hexBorder);
                hexBorderList.Add(border);
            }
            else
            {
                border = hexBorderList[i];
            }
            border.gameObject.SetActive(true);
            border.transform.position = grid.CellToWorld(posList[i]);
        }
    }

    public void hideBorder()
    {
        foreach (GameObject border in hexBorderList)
        {
            border.gameObject.SetActive(false);
        }
    }

    public void showRadius(AreaShape areaShape, int range)
    {
        //Vector3Int curPos = grid.WorldToCell(mCharacter.transform.position);
        Vector3Int curPos = PlayerManager.singleton.Player.currentTile;
        setTile(mTile, getPosList(areaShape, range, curPos));
    }

    public void hideRadius(AreaShape areaShape, int range)
    {
        //Vector3Int curPos = grid.WorldToCell(mCharacter.transform.position);
        Vector3Int curPos = PlayerManager.singleton.Player.currentTile;
        setTile(mOriginalTile, getPosList(areaShape, range, curPos));
    }

    public void setTile(Tile tile, List<Vector3Int> posList)
    {
        foreach (Vector3Int pos in posList)
        {
            if (tilemap.GetTile(pos) == null) continue;
            Color color = tilemap.GetColor(pos);
            tilemap.SetTile(pos, tile);
            tilemap.SetTileFlags(pos, TileFlags.None);
            tilemap.SetColor(pos, color);
        }
    }

    public void setTileColor(Color color, List<Vector3Int> posList)
    {
        foreach (Vector3Int pos in posList)
        {
            if (tilemap.GetTile(pos) == null) continue;
            tilemap.SetTileFlags(pos, TileFlags.None);
            tilemap.SetColor(pos, color);
        }
    }

    public List<Vector3Int> getPosListTarget(AreaShape areaShape, int range, Vector3Int curPos, Vector3 targetPos)
    {
        Vector3Int targetPosCell = grid.WorldToCell(targetPos);
        switch (areaShape)
        {
            case AreaShape.Cone:
                return getPosListCone(range, curPos, targetPos);
            default:
                return getPosList(areaShape, range, targetPosCell);
        }
    }

    public List<Vector3Int> getPosList(AreaShape areaShape, int range, Vector3Int curPos)
    {

        switch (areaShape)
        {
            case AreaShape.Single:
                return getPosListHexagon(0, curPos);
            case AreaShape.Hexagon:
                return getPosListHexagon(range, curPos);
            case AreaShape.Line:
                return getPosListLine(range, curPos);
            case AreaShape.Cone:
                return getPosListCone(range, curPos, grid.CellToWorld(curPos));
            default:
                return getPosListNear(curPos);
        }
    }

    private List<Vector3Int> getPosListHexagon(int range, Vector3Int curPos)
    {
        List<Vector3Int> posList = new List<Vector3Int>();
        int k = 1;
        if (Mathf.Abs(curPos.y) % 2 == 1) k = -1;
        for (int y = -range; y <= range; y++)
        {
            for (int x = -range; x <= range; x++)
            {
                int temp = x;
                if (Mathf.Abs(y) % 2 == 1 && k * x > 0) x += k;
                if (Mathf.Abs(x) + Mathf.Abs(y) / 2 <= range)
                {
                    x = temp;
                    posList.Add(new Vector3Int(x + curPos.x, y + curPos.y, 0));
                }
                x = temp;
            }
        }
        return posList;
    }

    public List<Vector3Int> getPosListNear(Vector3Int curPos)
    {
        List<Vector3Int> posList = new List<Vector3Int>();
        for (int i = 0; i < 6; i++)
        {
            posList.Add(getPosDirection(curPos, i));
        }
        return posList;
    }

    private List<Vector3Int> getPosListLine(int range, Vector3Int curPos)
    {
        List<Vector3Int> posList = new List<Vector3Int>();
        posList.Add(curPos);
        for (int i = 0; i < 6; i++)
        {
            posList.AddRange(getPosListDirection(range, curPos, i));
        }
        return posList;
    }

    public List<Vector3Int> getPosListDirection(int range, Vector3Int curPos, int direction)
    {
        List<Vector3Int> posList = new List<Vector3Int>();
        if (direction >= 0 && direction < 6)
        {
            Vector3Int pos = curPos;
            for (int i = 0; i < range; i++)
            {
                pos = getPosDirection(pos, direction);
                posList.Add(pos);
            }
        }
        return posList;
    }

    public Vector3Int getPosDirection(Vector3Int curPos, int direction)
    {
        Vector3Int pos = curPos;
        switch (direction)
        {
            // right
            case 0:
                pos.x += 1;
                return pos;
            // upper right
            case 1:
                pos.x += Mathf.Abs(pos.y) % 2;
                pos.y += 1;
                return pos;
            // upper left
            case 2:
                pos.x += Mathf.Abs(pos.y) % 2 - 1;
                pos.y += 1;
                return pos;
            // left
            case 3:
                pos.x -= 1;
                return pos;
            // lower left
            case 4:
                pos.x += Mathf.Abs(pos.y) % 2 - 1;
                pos.y -= 1;
                return pos;
            // lower right
            case 5:
                pos.x += Mathf.Abs(pos.y) % 2;
                pos.y -= 1;
                return pos;
            default:
                throw new Exception();
        }
    }

    public Vector2 getDirectionVector(int direction)
    {
        float radiant = direction * Mathf.PI / 3;
        return new Vector2(Mathf.Cos(radiant), Mathf.Sin(radiant));
    }

    public List<int> FindDirections(Vector3 oriPos, Vector3 targetPos)
    {
        Vector2 disPos = new Vector2(targetPos.x - oriPos.x, targetPos.y - oriPos.y);
        Dictionary<int, float> dict = new Dictionary<int, float>();
        List<int> direction = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            dict.Add(i, Vector2.Distance(getDirectionVector(i), disPos));
        }
        foreach (KeyValuePair<int, float> dis in dict.OrderBy(key => key.Value))
        {
            direction.Add(dis.Key);
        }
        return direction;
    }

    public List<Vector3Int> getPosListCone(int range, Vector3Int curPos, Vector3 targetPos)
    {
        int width = 2;
        Vector3 playerPos = PlayerManager.singleton.Player.transform.position;
        List<int> direction = Arena.singleton.FindDirections(playerPos, targetPos);
        List<Vector3Int> posList = new List<Vector3Int>();
        Queue<KeyValuePair<Vector3Int, int>> q = new Queue<KeyValuePair<Vector3Int, int>>();
        Vector3Int pos = curPos;
        q.Enqueue(new KeyValuePair<Vector3Int, int>(pos, 0));
        while (q.Count > 0)
        {
            KeyValuePair<Vector3Int, int> p = q.Dequeue();
            posList.Add(p.Key);
            if (p.Value < range)
            {
                for (int i = 0; i < width; i++)
                {
                    pos = getPosDirection(p.Key, direction[i]);
                    if (!posList.Contains(pos))
                    {
                        q.Enqueue(new KeyValuePair<Vector3Int, int>(pos, p.Value + 1));
                    }
                }
            }
        }
        return posList;
    }
}