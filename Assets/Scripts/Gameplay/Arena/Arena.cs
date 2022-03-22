using System;
using System.Collections;
using System.Collections.Generic;
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
    public GameObject redHexBorder;

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
        redHexBorder = transform.Find("redHexBorder").gameObject;
        BakeLineDebuger(redHexBorder);
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
            hexBorder.gameObject.SetActive(false);
            redHexBorder.gameObject.SetActive(true);
            redHexBorder.transform.position = grid.CellToWorld(mousePos);
        }
        else if (tile != null && tile.Equals(mTile))
        {
            redHexBorder.gameObject.SetActive(false);
            hexBorder.gameObject.SetActive(true);
            hexBorder.transform.position = grid.CellToWorld(mousePos);
        }
        else
        {
            hexBorder.gameObject.SetActive(false);
            redHexBorder.gameObject.SetActive(false);
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

    public List<Vector3Int> getPosList(AreaShape areaShape, int range, Vector3Int curPos)
    {
        switch (areaShape)
        {
            case AreaShape.Hexagon:
                return getPosListCircle(range, curPos);
            case AreaShape.Line:
                return getPosListLine(range, curPos);
            default:
                return getPosListNear(curPos);
        }
    }

    private List<Vector3Int> getPosListCircle(int range, Vector3Int curPos)
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
}