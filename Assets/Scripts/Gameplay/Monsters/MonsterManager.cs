using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour, ITurnHandler
{
    public static MonsterManager singleton;
    public Wave wave;
    public List<Monster> monsters = new List<Monster>();
    public bool isBusy = false;
    public Sprite SwordIcon;
    public Sprite BowIcon;
    [SerializeField] private GameObject monsterPrefab;
    private GridLayout grid;
    public List<Vector3Int> highlightedTiles = new List<Vector3Int>();
    private List<Vector3Int> highlightedTiles2 = new List<Vector3Int>();
    private Color redHighlight = new Color(1f, 0.8f, 0.8f);
    private Color redHighlight2 = new Color(1f, 0.5f, 0.5f);

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        grid = Arena.singleton.GetComponentInChildren<GridLayout>();
        SpawnWave();
    }

    void Update()
    {
        if (GameManager.gameState != GameState.Running) return;

        Arena.singleton.setTileColor(Color.white, highlightedTiles2);

        if (OptionMenu.showMonstersAttackArea)
        {
            Arena.singleton.setTileColor(redHighlight, highlightedTiles);
        }
        else Arena.singleton.setTileColor(Color.white, highlightedTiles);

        Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mousePos = grid.WorldToCell(new Vector3(oriPos.x, oriPos.y, 0));
        Monster monster = FindMonsterByTile(mousePos);
        if (monster != null)
        {
            highlightedTiles2 = monster.AttackArea();
            Arena.singleton.setTileColor(redHighlight2, highlightedTiles2);
        }
        else highlightedTiles2.Clear();
    }

    public void onStartTurn()
    {
        Arena.singleton.setTileColor(Color.white, highlightedTiles);
        highlightedTiles.Clear();

        foreach (Monster monster in monsters)
        {
            monster.Refresh();
            if (monster.ShowAttackArea())
                highlightedTiles.AddRange(monster.AttackArea());
        }
    }

    public void onEndTurn()
    {
        StartAttacking();
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

    public void StartAttacking()
    {
        StartCoroutine(Attack());
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

            monsters.Add(monster);
        }
    }
}
