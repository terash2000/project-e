using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager singleton;
    public Wave wave;
    [SerializeField] private GameObject monsterPrefab;
    private List<Monster> monsters = new List<Monster>();
    private GridLayout grid;

    public Vector3Int characterTile;

    void Awake(){
        singleton = this;
    }

    void Start()
    {
        grid = Arena.singleton.GetComponentInChildren<GridLayout>();
        SpawnWave();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) MoveMonsters();
    }

    public Monster FindMonsterByTile(Vector3Int tile)
    {
        foreach(Monster monster in monsters)
        {
            if(monster.currentTile == tile)
                return monster;
        }
        return null;
    }

    public void MoveMonsters()
    {
        foreach(Monster monster in monsters) monster.moved = false;
        foreach(Monster monster in monsters) monster.Move();
    }

    private void SpawnWave()
    {
        foreach(Wave.MonsterSpawner monsterSpawner in wave.monsters)
        {
            Monster monster = Instantiate(monsterPrefab, grid.transform).GetComponent<Monster>();
            monster.info = monsterSpawner.monster;
            monster.currentTile = new Vector3Int(monsterSpawner.tile.x ,monsterSpawner.tile.y, 0);

            monsters.Add(monster);
        }
    }
}
