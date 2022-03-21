using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, ITurnHandler
{
    public static PlayerManager singleton;
    public Player player = new Player();

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        
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
}
