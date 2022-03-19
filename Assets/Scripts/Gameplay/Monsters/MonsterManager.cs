using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour, ITurnHandler
{
    public static MonsterManager singleton;
    public Wave wave;
    public bool isBusy = false;
    [SerializeField] private GameObject monsterPrefab;
    private List<Monster> monsters = new List<Monster>();
    private GridLayout grid;
    private Color redHighlight = new Color(1f, 0.5f, 0.5f);

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
        //if(Input.GetKeyDown(KeyCode.Space) && !isBusy) StartAttacking();
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

    public void StartAttacking()
    {
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        isBusy = true;
        foreach(Monster monster in monsters)
        {
            if(monster.Attack()) yield return new WaitForSeconds(0.25f);
        }
        MoveMonsters();
        isBusy = false;
    }

    public void MoveMonsters()
    {
        foreach(Monster monster in monsters) monster.moved = false;
        foreach(Monster monster in monsters) monster.Move();

        HighlightAttackRange();
    }

    private void HighlightAttackRange()
    {
        foreach (Vector3Int tilePosition in Arena.singleton.tilemap.cellBounds.allPositionsWithin)
            Arena.singleton.tilemap.SetColor(tilePosition, Color.white);

        foreach(Monster monster in monsters)
            Arena.singleton.setTileColor(redHighlight, monster.AttackArea());
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

        HighlightAttackRange();
    }

    public void onStartTurn()
    {
        
    }

    public void onEndTurn()
    {
        MoveMonsters();
        GameManager.singleton.startTurn();
    }
}
