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

    private GridLayout _grid;
    private Tilemap _tilemap;
    private GameObject _hexBorder;
    private GameObject _redHexBorder;
    private List<Vector3Int> _areaPosList = new List<Vector3Int>();
    private List<Vector3Int> _targetPosList = new List<Vector3Int>();
    private List<Vector3Int> _monsterHighlight = new List<Vector3Int>();
    private List<Vector3Int> _monsterHighlight2 = new List<Vector3Int>();
    private Color _redHighlight = new Color(1f, 0.8f, 0.8f);
    private Color _redHighlight2 = new Color(1f, 0.5f, 0.5f);
    private Color _yellowHighlight = Color.yellow;

    public GridLayout Grid
    {
        get { return _grid; }
    }
    public Tilemap Tilemap
    {
        get { return _tilemap; }
    }
    public List<Vector3Int> AreaPosList
    {
        get { return _areaPosList; }
        set { _areaPosList = value; }
    }
    public List<Vector3Int> TargetPosList
    {
        get { return _targetPosList; }
        set { _targetPosList = value; }
    }
    public List<Vector3Int> MonsterHighlight
    {
        get { return _monsterHighlight; }
        set { _monsterHighlight = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _grid = GetComponentInChildren<GridLayout>();
        _tilemap = _grid.GetComponentInChildren<Tilemap>();

        _hexBorder = transform.Find("hexBorder").gameObject;
        BakeLineDebuger(_hexBorder);
        _hexBorder.gameObject.SetActive(false);

        _redHexBorder = transform.Find("redHexBorder").gameObject;
        BakeLineDebuger(_redHexBorder);
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
        if (GameManager.GameState != GameState.Running) return;

        Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mousePos = _grid.WorldToCell(new Vector3(oriPos.x, oriPos.y, 0));
        Monster monster = MonsterManager.Instance.FindMonsterByTile(mousePos);
        Tile tile = (Tile)_tilemap.GetTile(mousePos);

        // clear old highlight
        SetTileColor(Color.white, _monsterHighlight2);
        _monsterHighlight2.Clear();
        HideTargetArea();
        _redHexBorder.gameObject.SetActive(false);

        // monster highlight 1
        if (OptionMenu.ShowMonstersAttackArea)
        {
            SetTileColor(_redHighlight, _monsterHighlight);
        }
        else SetTileColor(Color.white, _monsterHighlight);

        if (CardManager.Instance.IsSelectingCard())
        {
            if ((_areaPosList.Contains(mousePos) && (CardManager.Instance.SelectingCard.Card.BaseCard.Type == CardType.Attack || monster == null))
                    || IsDirectionTarget(CardManager.Instance.SelectingCard.Card.BaseCard.TargetShape))
            {
                ShowTargetArea(CardManager.Instance.SelectingCard.Card, oriPos);
            }
        }
        else if (CardManager.Instance.IsHoveringCard() && IsDirectionTarget(CardManager.Instance.HoveringCard.Card.BaseCard.TargetShape))
        {
            ShowTargetArea(CardManager.Instance.HoveringCard.Card, oriPos);
        }
        else if (monster != null)
        {
            // monster highlight 2
            _redHexBorder.gameObject.SetActive(true);
            _redHexBorder.transform.position = _grid.CellToWorld(mousePos);

            _monsterHighlight2 = monster.AttackArea();
            Arena.Instance.SetTileColor(_redHighlight2, _monsterHighlight2);
        }
    }

    public void ShowTargetArea(InGameCard card, Vector3 targetPos)
    {
        _targetPosList = GetPosListTarget(card.BaseCard.TargetShape, card.Radius, PlayerManager.Instance.Player.CurrentTile, targetPos);
        SetTileColor(_yellowHighlight, _targetPosList);
    }

    public void HideTargetArea()
    {
        SetTileColor(Color.white, _targetPosList);
        _targetPosList = new List<Vector3Int>();
    }

    public void ShowRadius(InGameCard card)
    {
        Vector3Int curPos = PlayerManager.Instance.Player.CurrentTile;
        _hexBorder.gameObject.SetActive(true);
        _hexBorder.transform.position = _grid.CellToWorld(curPos);

        if (IsDirectionTarget(card.BaseCard.TargetShape)) return;

        _areaPosList = GetPosList(card.BaseCard.AreaShape, card.CastRange, curPos);
        _areaPosList.Remove(curPos);
        SetTile(mTile, _areaPosList);
    }

    public void HideRadius()
    {
        Vector3Int curPos = PlayerManager.Instance.Player.CurrentTile;
        _hexBorder.gameObject.SetActive(false);
        SetTile(mOriginalTile, _areaPosList);
        _areaPosList.Clear();
    }

    public void SetTile(Tile tile, List<Vector3Int> posList)
    {
        foreach (Vector3Int pos in posList)
        {
            if (_tilemap.GetTile(pos) == null) continue;
            Color color = _tilemap.GetColor(pos);
            _tilemap.SetTile(pos, tile);
            _tilemap.SetTileFlags(pos, TileFlags.None);
            _tilemap.SetColor(pos, color);
        }
    }

    public void SetTileColor(Color color, List<Vector3Int> posList)
    {
        foreach (Vector3Int pos in posList)
        {
            if (_tilemap.GetTile(pos) == null) continue;
            _tilemap.SetTileFlags(pos, TileFlags.None);
            _tilemap.SetColor(pos, color);
        }
    }

    public void RemoveMonsterHighlight(List<Vector3Int> posList)
    {
        Arena.Instance.SetTileColor(Color.white, posList);
        foreach (Vector3Int pos in new List<Vector3Int>(posList))
        {
            Arena.Instance._monsterHighlight.Remove(pos);
        }
    }

    public List<Vector3Int> GetPosListTarget(AreaShape areaShape, int range, Vector3Int curPos, Vector3 targetPos)
    {
        Vector3 playerPos = _grid.CellToWorld(PlayerManager.Instance.Player.CurrentTile);
        List<int> directions = Arena.Instance.FindDirections(playerPos, targetPos);
        switch (areaShape)
        {
            case AreaShape.Line:
                return GetPosListDirection(range, curPos, directions[0]);
            case AreaShape.Cone:
                return GetPosListCone(range, curPos, directions);
            default:
                return GetPosList(areaShape, range, _grid.WorldToCell(targetPos));
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

    public List<Vector3Int> GetPosListBeam(int range, Vector3Int curPos, int direction)
    {
        List<Vector3Int> posList = new List<Vector3Int>();
        posList = posList.Concat(GetPosListDirection(range, curPos, direction)).ToList();
        posList = posList.Concat(GetPosListDirection(range - 1, GetPosDirection(curPos, (direction + 1) % 6), direction)).ToList();
        posList = posList.Concat(GetPosListDirection(range - 1, GetPosDirection(curPos, (direction + 5) % 6), direction)).ToList();
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
            if (!posList.Contains(p.Key)) posList.Add(p.Key);
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