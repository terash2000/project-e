using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    [HideInInspector]
    public int round;
    [HideInInspector]
    public bool playerTurn;
    //public Arena mArena;

    public GameState gameState;

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        round = 0;
        gameState = GameState.Running;
        startTurn();
    }

    void Update()
    {
        if (PlayerData.health <= 0 && gameState == GameState.Running)
        {
            GameResultPopup gameResultPopup = Resources.FindObjectsOfTypeAll<GameResultPopup>()[0];
            gameResultPopup.onLose();
            gameState = GameState.Lose;
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
        while (MonsterManager.singleton.isBusy)
            yield return new WaitForSeconds(Time.deltaTime);
        startTurn();
    }


}