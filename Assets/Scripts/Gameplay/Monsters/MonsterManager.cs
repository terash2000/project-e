using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour, ITurnHandler
{
    public static MonsterManager singleton;
    public static Wave wave;
    public List<Monster> monsters = new List<Monster>();
    public bool isBusy = false;
    public Sprite SwordIcon;
    public Sprite BowIcon;
    [SerializeField] private GameObject monsterPrefab;
    private GridLayout grid;

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        grid = Arena.singleton.GetComponentInChildren<GridLayout>();
        SpawnWave();
    }

    public void onStartTurn()
    {
        Arena.singleton.removeMonsterHighlight(Arena.singleton.monsterHighlight);

        foreach (Monster monster in monsters)
        {
            monster.Refresh();
            if (monster.ShowAttackArea())
                Arena.singleton.monsterHighlight.AddRange(monster.AttackArea());
        }
    }

    public void onEndTurn()
    {
        foreach (Monster monster in new List<Monster>(monsters))
        {
            monster.TriggerStatus();
        }
        StartCoroutine(Attack());
    }

    public Monster FindMonsterByTile(Vector3Int tile)
    {
        foreach (Monster monster in monsters)
        {
            if (monster.currentTile == tile)
                return monster;
        }
        return null;
    }

    private IEnumerator Attack()
    {
        isBusy = true;
        foreach (Monster monster in monsters)
        {
            if (monster.Attack()) yield return new WaitForSeconds(0.15f);
        }
        MoveMonsters();
        isBusy = false;
    }

    public void MoveMonsters()
    {
        foreach (Monster monster in monsters)
        {
            if (!monster.attacked || monster.CanMoveAfterAttack()) monster.Move();
        }
    }

    private void SpawnWave()
    {
        foreach (Wave.MonsterSpawner monsterSpawner in wave.monsters)
        {
            Monster monster = Instantiate(monsterPrefab, grid.transform).GetComponent<Monster>();
            monster.info = monsterSpawner.monster;
            monster.currentTile = new Vector3Int(monsterSpawner.tile.x, monsterSpawner.tile.y, 0);
            monster.currentMove = monsterSpawner.currentMove;

            monsters.Add(monster);
        }
    }
}
