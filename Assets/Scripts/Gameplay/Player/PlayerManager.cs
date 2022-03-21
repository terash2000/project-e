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

    }

    public void onStartTurn()
    {

    }

    public void onEndTurn()
    {

    }

    public void TakeDamage(int damage)
    {
        PlayerData.health -= damage;
        if (PlayerData.health < 0) PlayerData.health = 0;
    }
}
