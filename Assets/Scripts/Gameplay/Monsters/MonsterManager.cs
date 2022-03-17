using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager singleton;
    public Wave wave;
    [SerializeField] private GridLayout mGrid;
    [SerializeField] private GameObject monsterPrefab;
    private List<Monster> monsters = new List<Monster>();

    void Awake(){
        singleton = this;
    }

    void Start()
    {
        SpawnWave();
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

    private void SpawnWave()
    {
        foreach(Wave.MonsterSpawner monsterSpawner in wave.monsters)
        {
            Monster monster = Instantiate(monsterPrefab, mGrid.transform).GetComponent<Monster>();
            monster.info = monsterSpawner.monster;
            monster.mGrid = mGrid;
            monster.currentTile = new Vector3Int(monsterSpawner.tile.x ,monsterSpawner.tile.y, 0);

            monsters.Add(monster);
        }
    }
}
