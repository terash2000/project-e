using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviourSingleton<MonsterManager>, ITurnHandler
{
    private static Wave _wave;

    public Sprite SwordIcon;
    public Sprite BowIcon;
    public Sprite SwordAndShieldIcon;
    public Sprite StunIcon;
    [SerializeField] private GameObject _monsterPrefab;
    private GridLayout _grid;
    private List<Monster> _monsters = new List<Monster>();
    private bool _isBusy = false;

    public static Wave Wave
    {
        get { return _wave; }
        set
        { _wave = value; }
    }
    public List<Monster> Monsters
    {
        get { return _monsters; }
        set { _monsters = value; }
    }
    public bool IsBusy
    {
        get { return _isBusy; }
        set { _isBusy = value; }
    }


    void Start()
    {
        _grid = Arena.Instance.GetComponentInChildren<GridLayout>();
        SpawnWave();
    }

    public void OnStartTurn()
    {
        Arena.Instance.RemoveMonsterHighlight(Arena.Instance.monsterHighlight);

        foreach (Monster monster in Monsters)
        {
            monster.Refresh();
            if (monster.ShowAttackArea())
                Arena.Instance.monsterHighlight.AddRange(monster.AttackArea());
        }
    }

    public void OnEndTurn()
    {
        foreach (Monster monster in new List<Monster>(Monsters))
        {
            monster.TriggerStatus();
        }
        StartCoroutine(Attack());
    }

    public Monster FindMonsterByTile(Vector3Int tile)
    {
        foreach (Monster monster in Monsters)
        {
            if (monster.CurrentTile == tile)
                return monster;
        }
        return null;
    }

    private IEnumerator Attack()
    {
        _isBusy = true;
        foreach (Monster monster in Monsters)
        {
            if (monster.Attack()) yield return new WaitForSeconds(0.15f);
        }
        MoveMonsters();
        _isBusy = false;
    }

    public void MoveMonsters()
    {
        foreach (Monster monster in Monsters)
        {
            if (!monster.HasAttacked || monster.CanMoveAfterAttack()) monster.Move();
        }
    }

    private void SpawnWave()
    {
        if (Wave != null)
        {
            foreach (Wave.MonsterSpawner monsterSpawner in Wave.monsters)
            {
                Monster monster = Instantiate(_monsterPrefab, _grid.transform).GetComponent<Monster>();
                monster.Info = monsterSpawner.monster;
                monster.CurrentTile = new Vector3Int(monsterSpawner.tile.x, monsterSpawner.tile.y, 0);
                monster.CurrentMove = monsterSpawner.currentMove;

                Monsters.Add(monster);
            }
        }
    }
}
