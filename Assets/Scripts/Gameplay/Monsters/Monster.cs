using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MoveableSprite
{
    public MonsterInfo info;
    public GameObject healthBar;
    public GameObject healthText;
    public GameObject damageText;
    public bool moved;
    public bool attacked;

    private int healthAmount;
    private Vector3 healthLocalScale;
    private float healthBarSize;
    private int attackDirection = -1;
    private float radiant2 = 0f;

    protected override void Start()
    {
        base.Start();

        animator.runtimeAnimatorController = info.animatorController;
        GetComponent<SpriteRenderer>().color = info.spriteColor;

        healthAmount = info.maxHealth;
        healthLocalScale = healthBar.transform.localScale;
        healthBarSize = healthLocalScale.x;
    }

    protected override void Update()
    {
        base.Update();

        // attack animation
        if (radiant2 != 0f)
        {
            Vector3 characterPos = grid.CellToWorld(PlayerManager.singleton.Player.currentTile);
            Vector3 attackDirection = characterPos - transform.position;
            attackDirection.Normalize();
            transform.position = transform.position + attackDirection * 0.25f * Mathf.Sin(radiant2);
        }

        healthLocalScale.x = (float)healthAmount / (float)info.maxHealth * healthBarSize;
        healthBar.transform.localScale = healthLocalScale;
        healthText.GetComponent<TMPro.TextMeshProUGUI>().text = healthAmount.ToString();

        damageText.GetComponent<TMPro.TextMeshProUGUI>().text = info.patterns[0].damage.ToString();
    }

    public List<Vector3Int> AttackArea()
    {
        List<Vector3Int> area = new List<Vector3Int>();

        switch (info.patterns[0].pattern)
        {
            case MonsterPatternType.Basic:
                area.AddRange(Arena.singleton.getPosListNear(currentTile));
                break;
            case MonsterPatternType.Range:
                area.AddRange(Arena.singleton.getPosListDirection(2, currentTile, attackDirection));
                break;
        }

        return area;
    }

    public void Move()
    {
        if (moved) return;
        moved = true;

        Vector3Int characterTile = PlayerManager.singleton.Player.currentTile;
        List<Vector3Int> targetTiles = new List<Vector3Int>();
        List<Vector3Int> moveableTiles = new List<Vector3Int> { currentTile };

        switch (info.patterns[0].pattern)
        {
            case MonsterPatternType.Basic:
            case MonsterPatternType.Range:
                moveableTiles.AddRange(Arena.singleton.getPosListNear(currentTile));
                break;
        }

        moveableTiles = moveableTiles.FindAll(tile => (
            tile != characterTile &&
            Arena.singleton.tilemap.GetTile(tile) != null
        ));

        while (moveableTiles.Count != 0)
        {
            // choose target tiles to move
            switch (info.patterns[0].pattern)
            {
                case MonsterPatternType.Basic:
                    targetTiles = ShortenDistance(moveableTiles, characterTile);
                    break;
                case MonsterPatternType.Range:
                    targetTiles = StayDistance(2, moveableTiles, characterTile);
                    break;
            }

            if (MoveIfEmpty(targetTiles)) break;

            // try to move nearby monster
            foreach (Vector3Int tile in targetTiles)
            {
                moveableTiles.Remove(tile);

                Monster nearbyMonster = MonsterManager.singleton.FindMonsterByTile(tile);
                if (nearbyMonster != null) nearbyMonster.Move();
            }

            if (MoveIfEmpty(targetTiles)) break;
            if (targetTiles.Contains(currentTile)) break;
        }
    }

    public void Refresh()
    {
        moved = false;
        attacked = false;
        attackDirection = CalAttackDirection();
    }

    private int CalAttackDirection()
    {
        Vector3Int characterTile = PlayerManager.singleton.Player.currentTile;

        switch (info.patterns[0].pattern)
        {
            case MonsterPatternType.Range:
                for (int i = 0; i < 6; i++)
                {
                    List<Vector3Int> attackableArea = Arena.singleton.getPosListDirection(2, currentTile, i);
                    if (attackableArea.Contains(characterTile)) return i;
                    if (Arena.singleton.getPosListNear(attackableArea[1]).Contains(characterTile))
                    {
                        if (CalDistance(currentTile, characterTile) > 2)
                            return i;
                        return Random.Range(i, i + 2) % 6;
                    }
                }
                return -1;
            default:
                return -1;
        }
    }

    public bool Attack()
    {
        Vector3Int characterTile = PlayerManager.singleton.Player.currentTile;
        if (AttackArea().Contains(characterTile))
        {
            PlayerData.health -= info.patterns[0].damage;
            StartCoroutine(AttackAnimation());
            attacked = true;
            return true;
        }
        return false;
    }

    private IEnumerator AttackAnimation()
    {
        radiant2 = Mathf.PI;
        while (radiant2 > 0f)
        {
            radiant2 -= Mathf.PI * 5 * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        radiant2 = 0f;
    }

    public int TakeDamage(int damage)
    {
        healthAmount -= damage;
        if (healthAmount <= 0)
        {
            Die();
            return 0;
        }
        return healthAmount;
    }

    public bool CanMoveAfterAttack()
    {
        return info.patterns.Count > 1;
    }

    public bool ShowAttackArea()
    {
        return info.patterns[0].pattern != MonsterPatternType.Basic;
    }

    private void Die()
    {
        MonsterManager.singleton.monsters.Remove(this);
        RemoveHighlight();
        Destroy(gameObject);
    }

    private void RemoveHighlight()
    {
        if (ShowAttackArea())
        {
            foreach (Vector3Int tile in AttackArea())
            {
                MonsterManager.singleton.highlightedTiles.Remove(tile);
            }
        }
    }

    private int CalDistance(Vector3Int tile1, Vector3Int tile2)
    {
        float x1 = (float)tile1.x + 0.5f * (Mathf.Abs(tile1.y) % 2);
        float x2 = (float)tile2.x + 0.5f * (Mathf.Abs(tile2.y) % 2);
        float dy = Mathf.Abs((float)tile1.y - (float)tile2.y);
        float dx = Mathf.Abs(x1 - x2);
        int distance = (int)(dy + Mathf.Max(dx - dy / 2, 0));
        return distance;
    }

    private List<Vector3Int> StayDistance(int idealDistance, List<Vector3Int> moveableTiles, Vector3Int characterTile)
    {
        List<Vector3Int> targetTiles = new List<Vector3Int>();
        int minDistance = int.MaxValue;

        foreach (Vector3Int tile in moveableTiles)
        {
            int distance = Mathf.Abs(CalDistance(tile, characterTile) - idealDistance);
            if (distance < minDistance)
            {
                minDistance = distance;
                targetTiles = new List<Vector3Int> { tile };
            }
            else if (distance == minDistance) targetTiles.Add(tile);
        }
        return targetTiles;
    }

    private List<Vector3Int> ShortenDistance(List<Vector3Int> moveableTiles, Vector3Int characterTile)
    {
        return StayDistance(0, moveableTiles, characterTile);
    }

    private bool MoveIfEmpty(List<Vector3Int> targetTiles)
    {
        List<Vector3Int> targetEmptyTiles = targetTiles.FindAll(tile =>
            MonsterManager.singleton.FindMonsterByTile(tile) == null
        );
        if (targetEmptyTiles.Count != 0)
        {
            MoveRandom(targetEmptyTiles);
            return true;
        }
        return false;
    }

    private void MoveRandom(List<Vector3Int> targetTiles)
    {
        Vector3Int destination = targetTiles[Random.Range(0, targetTiles.Count)];
        SetMovement(destination);
    }
}
