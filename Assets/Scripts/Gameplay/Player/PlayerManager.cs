using UnityEngine;

public class PlayerManager : MonoBehaviourSingleton<PlayerManager>, ITurnHandler
{
    private const float PLAYER_SCALE = 0.75f;

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
        _player.transform.localScale = new Vector3(PLAYER_SCALE, PLAYER_SCALE, 1);
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
