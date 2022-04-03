using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public static GameState gameState;
    [HideInInspector]
    public int round;
    [HideInInspector]
    public bool playerTurn;
    public Sprite acidIcon;
    public Color acidColor;
    public Sprite burnIcon;
    public Color burnColor;
    public Color blockColor;

    public GameResultPopup gameResultPopup;

    void Start()
    {
        SaveSystem.LoadOptionMenu();

        round = 0;
        gameState = GameState.Running;

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
        if (gameState == GameState.Running)
        {
            if (PlayerData.Health <= 0)
            {
                SaveSystem.DeleteSave();
                gameResultPopup.OnLose();
                gameState = GameState.Lose;
            }
            else if (MonsterManager.Instance.monsters.Count == 0)
            {
                gameResultPopup.OnWin();
                gameState = GameState.Win;
            }
        }
    }

    public void StartTurn()
    {
        round++;
        playerTurn = true;
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
        playerTurn = false;
        var turnHandlerObjects = FindObjectsOfType<MonoBehaviour>().OfType<ITurnHandler>();
        foreach (ITurnHandler turnHandlerObject in turnHandlerObjects)
        {
            turnHandlerObject.OnEndTurn();
        }
        while (MonsterManager.Instance.isBusy)
            yield return new WaitForEndOfFrame();
        StartTurn();
    }
}