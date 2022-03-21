using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, ITurnHandler
{
    public static PlayerManager singleton;
    private Player _player;
    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }
    private GridLayout _grid;
    private List<Vector3Int> _highlightedTiles = new List<Vector3Int>();
    private List<Vector3Int> _highlightedTiles2 = new List<Vector3Int>();
    private Color _blueHighlight = new Color(0.5f, 0.5f, 1f);
    private Color _blueHighlight2 = new Color(0.8f, 0.8f, 1f);

    [SerializeField] private GameObject _playerPrefabs;

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        _grid = Arena.singleton.GetComponentInChildren<GridLayout>();
        _player = Instantiate(_playerPrefabs, _grid.transform).GetComponent<Player>();
    }

    void Update()
    {
        Arena.singleton.setTileColor(Color.white, _highlightedTiles);
        Arena.singleton.setTileColor(_blueHighlight2, _highlightedTiles2);

        Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mousePos = _grid.WorldToCell(new Vector3(oriPos.x, oriPos.y, 0));
        if (_player.currentTile == mousePos)
        {
            _highlightedTiles = _player.AttackArea();
            Arena.singleton.setTileColor(_blueHighlight, _highlightedTiles);
        }
        else _highlightedTiles.Clear();
    }

    public void onStartTurn()
    {

    }

    public void onEndTurn()
    {

    }
}
