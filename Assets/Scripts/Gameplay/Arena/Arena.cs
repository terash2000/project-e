using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Arena : MonoBehaviourSingleton<Arena>
{
    private const int TOTAL_DIRECTION = 6;

    public Tile mTile;
    public Tile mOriginalTile;
    [HideInInspector]
    public GridLayout grid;
    [HideInInspector]
    public Tilemap tilemap;
    [HideInInspector]
    public GameObject hexBorder;
    [HideInInspector]
    public List<Vector3Int> AreaPosList;
    [HideInInspector]
    public List<Vector3Int> TargetPosList;
    [HideInInspector]
    public GameObject redHexBorder;
    [HideInInspector]
    public CardController SelectedCard;
    [HideInInspector]
    public List<Vector3Int> monsterHighlight = new List<Vector3Int>();
    [HideInInspector]
    public List<Vector3Int> monsterHighlight2 = new List<Vector3Int>();

    private Color redHighlight = new Color(1f, 0.8f, 0.8f);
    private Color redHighlight2 = new Color(1f, 0.5f, 0.5f);

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
        meshRenderer.gameObject.SetActive(false);

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
        Monster monster = MonsterManager.Instance.FindMonsterByTile(mousePos);
        Tile tile = (Tile)tilemap.GetTile(mousePos);

        // clear old highlight
        SetTileColor(Color.white, monsterHighlight2);
        monsterHighlight2.Clear();
        HideTargetArea();
        redHexBorder.gameObject.SetActive(false);

        // highlight monster 1
        if (OptionMenu.ShowMonstersAttackArea)
        {
            SetTileColor(redHighlight, monsterHighlight);
        }
        else SetTileColor(Color.white, monsterHighlight);

        if (tile != null && AreaPosList.Contains(mousePos))
        {
            // highlight card
            ShowTargetArea(oriPos);
        }
        else if (monster != null)
        {
            // highlight monster 2
            redHexBorder.gameObject.SetActive(true);
            redHexBorder.transform.position = grid.CellToWorld(mousePos);

            monsterHighlight2 = monster.AttackArea();
            Arena.Instance.SetTileColor(redHighlight2, monsterHighlight2);
        }

        if (SelectedCard != null && IsDirectionTarget(SelectedCard.mTargetShape))
        {
            ShowTargetArea(oriPos);
        }
    }

    public void ShowTargetArea(Vector3 targetPos)
    {
        TargetPosList = GetPosListTarget(SelectedCard.mTargetShape, SelectedCard.mRange, PlayerManager.Instance.Player.CurrentTile, targetPos);
        SetTileColor(Color.yellow, TargetPosList);
    }

    public void HideTargetArea()
    {
        SetTileColor(Color.white, TargetPosList);
    }

    public void ShowRadius(AreaShape areaShape, AreaShape targetShape, int range)
    {
        Vector3Int curPos = PlayerManager.Instance.Player.CurrentTile;
        hexBorder.gameObject.SetActive(true);
        hexBorder.transform.position = grid.CellToWorld(curPos);
        AreaPosList = GetPosList(areaShape, range, curPos);
        AreaPosList.Remove(curPos);
        if (!IsDirectionTarget(targetShape)) SetTile(mTile, AreaPosList);

        HideTargetArea();
        ShowTargetArea(grid.CellToWorld(curPos));
    }

    public void HideRadius(AreaShape areaShape, int range)
    {
        Vector3Int curPos = PlayerManager.Instance.Player.CurrentTile;
        hexBorder.gameObject.SetActive(false);
        AreaPosList.Clear();
        SetTile(mOriginalTile, GetPosList(areaShape, range, curPos));
    }

    public void SetTile(Tile tile, List<Vector3Int> posList)
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

    public void SetTileColor(Color color, List<Vector3Int> posList)
    {
        foreach (Vector3Int pos in posList)
        {
            if (tilemap.GetTile(pos) == null) continue;
            tilemap.SetTileFlags(pos, TileFlags.None);
            tilemap.SetColor(pos, color);
        }
    }

    public void RemoveMonsterHighlight(List<Vector3Int> posList)
    {
        Arena.Instance.SetTileColor(Color.white, posList);
        foreach (Vector3Int pos in new List<Vector3Int>(posList))
        {
            Arena.Instance.monsterHighlight.Remove(pos);
        }
    }

    public List<Vector3Int> GetPosListTarget(AreaShape areaShape, int range, Vector3Int curPos, Vector3 targetPos)
    {
        Vector3 playerPos = PlayerManager.Instance.Player.transform.position;
        List<int> directions = Arena.Instance.FindDirections(playerPos, targetPos);
        switch (areaShape)
        {
            case AreaShape.Line:
                return GetPosListDirection(range, curPos, directions[0]);
            case AreaShape.Cone:
                return GetPosListCone(range, curPos, directions);
            default:
                return GetPosList(areaShape, range, grid.WorldToCell(targetPos));
        }
    }

    public List<Vector3Int> GetPosList(AreaShape areaShape, int range, Vector3Int curPos)
    {

        switch (areaShape)
        {
            case AreaShape.Single:
                return GetPosListHexagon(0, curPos);
            case AreaShape.Hexagon:
                return GetPosListHexagon(range, curPos);
            case AreaShape.Line:
                return GetPosListLine(range, curPos);
            case AreaShape.Cone:
                return GetPosListCone(range, curPos);
            default:
                return GetPosListNear(curPos);
        }
    }

    private List<Vector3Int> GetPosListHexagon(int range, Vector3Int curPos)
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

    public List<Vector3Int> GetPosListNear(Vector3Int curPos)
    {
        List<Vector3Int> posList = new List<Vector3Int>();
        for (int i = 0; i < TOTAL_DIRECTION; i++)
        {
            posList.Add(GetPosDirection(curPos, i));
        }
        return posList;
    }

    private List<Vector3Int> GetPosListLine(int range, Vector3Int curPos)
    {
        List<Vector3Int> posList = new List<Vector3Int>();
        posList.Add(curPos);
        for (int i = 0; i < TOTAL_DIRECTION; i++)
        {
            posList.AddRange(GetPosListDirection(range, curPos, i));
        }
        return posList;
    }

    public List<Vector3Int> GetPosListDirection(int range, Vector3Int curPos, int direction)
    {
        List<Vector3Int> posList = new List<Vector3Int>();
        if (direction >= 0 && direction < TOTAL_DIRECTION)
        {
            Vector3Int pos = curPos;
            for (int i = 0; i < range; i++)
            {
                pos = GetPosDirection(pos, direction);
                posList.Add(pos);
            }
        }
        return posList;
    }

    public Vector3Int GetPosDirection(Vector3Int curPos, int direction)
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

    public Vector2 GetDirectionVector(int direction)
    {
        float radiant = direction * Mathf.PI / 3;
        return new Vector2(Mathf.Cos(radiant), Mathf.Sin(radiant));
    }

    public List<int> FindDirections(Vector3 oriPos, Vector3 targetPos)
    {
        Vector2 disPos = new Vector2(targetPos.x - oriPos.x, targetPos.y - oriPos.y);
        Dictionary<int, float> dict = new Dictionary<int, float>();
        List<int> directions = new List<int>();
        for (int i = 0; i < TOTAL_DIRECTION; i++)
        {
            dict.Add(i, Vector2.Distance(GetDirectionVector(i), disPos));
        }
        foreach (KeyValuePair<int, float> dis in dict.OrderBy(key => key.Value))
        {
            directions.Add(dis.Key);
        }
        return directions;
    }

    public List<Vector3Int> GetPosListCone(int range, Vector3Int curPos, List<int> directions = null)
    {
        int width = 2;
        if (directions == null)
        {
            directions = FindDirections(curPos, curPos);
        }
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
                    pos = GetPosDirection(p.Key, directions[i]);
                    if (!posList.Contains(pos))
                    {
                        q.Enqueue(new KeyValuePair<Vector3Int, int>(pos, p.Value + 1));
                    }
                }
            }
        }
        posList.Remove(curPos);
        return posList;
    }

    public bool IsDirectionTarget(AreaShape targetShape)
    {
        return targetShape == AreaShape.Line || targetShape == AreaShape.Cone;
    }
}