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
        startTurn();
    }

    void Update()
    {
        if (gameState == GameState.Running)
        {
            if (PlayerData.Health <= 0)
            {
                SaveSystem.DeleteSave();
                gameResultPopup.onLose();
                gameState = GameState.Lose;
            }
            else if (MonsterManager.Instance.monsters.Count == 0)
            {
                gameResultPopup.onWin();
                gameState = GameState.Win;
            }
        }
    }

    public void startTurn()
    {
        round++;
        playerTurn = true;
        var turnHandlerObjects = FindObjectsOfType<MonoBehaviour>().OfType<ITurnHandler>();
        foreach (ITurnHandler turnHandlerObject in turnHandlerObjects)
        {
            turnHandlerObject.onStartTurn();
        }
    }

    public void endTurn()
    {
        StartCoroutine(doeEndTurn());
    }

    private IEnumerator doeEndTurn()
    {
        playerTurn = false;
        var turnHandlerObjects = FindObjectsOfType<MonoBehaviour>().OfType<ITurnHandler>();
        foreach (ITurnHandler turnHandlerObject in turnHandlerObjects)
        {
            turnHandlerObject.onEndTurn();
        }
        while (MonsterManager.Instance.isBusy)
            yield return new WaitForEndOfFrame();
        startTurn();
    }
}