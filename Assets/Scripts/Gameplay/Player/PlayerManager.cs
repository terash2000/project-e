using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, ITurnHandler
{
    public static PlayerManager singleton;
    public Player player;
    private GridLayout grid;

    [SerializeField] private GameObject playerPrefabs;

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        player = gameObject.AddComponent<Player>();

        grid = Arena.singleton.GetComponentInChildren<GridLayout>();

        player = Instantiate(playerPrefabs, grid.transform).GetComponent<Player>();
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
