using UnityEngine;

public class PlayerManager : MonoBehaviourSingleton<PlayerManager>, ITurnHandler
{
    [SerializeField]
    private GameObject _playerPrefabs;

    private Player _player;

    private GridLayout _grid;

    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }

    void Start()
    {
        _grid = Arena.Instance.GetComponentInChildren<GridLayout>();
        _player = Instantiate(_playerPrefabs, _grid.transform).GetComponent<Player>();

        // set local scale
        _player.transform.localScale = new Vector3(0.75f, 0.75f, 1);
    }

    void Update()
    {

    }

    public void OnStartTurn()
    {

    }

    public void OnEndTurn()
    {

    }
}
