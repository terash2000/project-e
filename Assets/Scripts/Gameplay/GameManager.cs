using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    private static GameState _gameState;
    private static bool _playerTurn;
    private static int _round;

    public Sprite AcidIcon;
    public Color AcidColor;
    public Sprite BurnIcon;
    public Color BurnColor;
    public GameResultPopup GameResultPopup;

    public static GameState GameState
    {
        get { return _gameState; }
        set { _gameState = value; }
    }
    public static bool PlayerTurn
    {
        get { return _playerTurn; }
        set { _playerTurn = value; }
    }
    public static int Round
    {
        get { return _round; }
    }

    void Start()
    {
        SaveSystem.LoadOptionMenu();

        _round = 0;
        _gameState = GameState.Running;

        // start after init other object
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        StartTurn();
    }

    void Update()
    {
        if (_gameState == GameState.Running)
        {
            if (PlayerData.Health <= 0)
            {
                SaveSystem.DeleteSave();
                GameResultPopup.OnLose();
                _gameState = GameState.Lose;
            }
            else if (MonsterManager.Instance.Monsters.Count == 0)
            {
                GameResultPopup.OnWin();
                _gameState = GameState.Win;
            }
        }
    }

    public void StartTurn()
    {
        _round++;
        _playerTurn = true;
        var turnHandlerObjects = FindObjectsOfType<MonoBehaviour>().OfType<ITurnHandler>();
        foreach (ITurnHandler turnHandlerObject in turnHandlerObjects)
        {
            turnHandlerObject.OnStartTurn();
        }
    }

    public void EndTurn()
    {
        StartCoroutine(DoeEndTurn());
    }

    private IEnumerator DoeEndTurn()
    {
        _playerTurn = false;
        var turnHandlerObjects = FindObjectsOfType<MonoBehaviour>().OfType<ITurnHandler>();
        foreach (ITurnHandler turnHandlerObject in turnHandlerObjects)
        {
            turnHandlerObject.OnEndTurn();
        }
        while (MonsterManager.Instance.IsBusy)
            yield return new WaitForEndOfFrame();
        StartTurn();
    }
}