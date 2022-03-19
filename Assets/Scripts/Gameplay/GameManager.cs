using UnityEngine;
using System.Collections.Generic;
using System.Linq; 

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    //[HideInInspector]
    public int round;
    [HideInInspector]
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
            Debug.Log(1);
        }
    }

    public void endTurn()
    {
        playerTurn = false;
        var turnHandlerObjects = FindObjectsOfType<MonoBehaviour>().OfType<ITurnHandler>();
        foreach (ITurnHandler turnHandlerObject in turnHandlerObjects) {
            turnHandlerObject.onEndTurn();
        }
    }


}