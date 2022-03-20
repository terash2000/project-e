using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    //[HideInInspector]
    public int round;
    //[HideInInspector]
    public bool playerTurn;
    //public Arena mArena;

    void Awake(){
        singleton = this;
    }

    void Start()
    {
        round = 0;
        startTurn();
    }

    public void startTurn()
    {
        round++;
        playerTurn = true;
        var turnHandlerObjects = FindObjectsOfType<MonoBehaviour>().OfType<ITurnHandler>();
        foreach (ITurnHandler turnHandlerObject in turnHandlerObjects) {
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
        foreach (ITurnHandler turnHandlerObject in turnHandlerObjects) {
            turnHandlerObject.onEndTurn();
        }
        yield return new WaitForSeconds(1f);
        startTurn();
    }


}