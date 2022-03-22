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
    private bool stuned;
    private int healthAmount;
    private Vector3 healthLocalScale;
    private float healthBarSize;
    private int attackDirection = -1;
    private float radiant2 = 0f;

    protected override void Start()
    {
        base.Start();

        animator.runtimeAnimatorController = info.animatorController;
        sprite.GetComponent<SpriteRenderer>().color = info.spriteColor;
        sprite.transform.localScale = new Vector3(info.spriteScale, info.spriteScale, transform.localScale.z);

        // look at center of areana
        Vector3 direction = -transform.position;
        direction.Normalize();
        lookDirection = direction;

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
            animator.SetBool("Attacking", true);
            Vector3 characterPos = grid.CellToWorld(PlayerManager.singleton.Player.currentTile);
            Vector3 direction = characterPos - transform.position;
            direction.Normalize();
            transform.position = transform.position + direction * 0.25f * Mathf.Sin(radiant2);
            lookDirection = direction;
        }
        else animator.SetBool("Attacking", false);

        if (!IsMoving() && attackDirection != -1)
        {
            lookDirection = Arena.singleton.getDirectionVector(attackDirection);
        }

        healthLocalScale.x = (float)healthAmount / (float)info.maxHealth * healthBarSize;
        healthBar.transform.localScale = healthLocalScale;
        healthText.GetComponent<TMPro.TextMeshProUGUI>().text = healthAmount.ToString();

        int damage = stuned ? 0 : info.patterns[0].damage;
        damageText.GetComponent<TMPro.TextMeshProUGUI>().text = damage.ToString();
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

    public void Stun()
    {
        RemoveHighlight();
        stuned = true;
        animator.SetBool("Stuned", true);
    }

    public List<Vector3Int> AttackArea()
    {
        List<Vector3Int> area = new List<Vector3Int>();
        if (stuned) return area;

        MonsterInfo.MonsterPattern pattern = info.patterns[0];
        switch (pattern.pattern)
        {
            case MonsterPatternType.Basic:
                area.AddRange(Arena.singleton.getPosListNear(currentTile));
                break;
            case MonsterPatternType.Range:
                area.AddRange(Arena.singleton.getPosListDirection(pattern.attackRange, currentTile, attackDirection));
                break;
        }

        return area;
    }

    public void Move()
    {
        if (moved || stuned) return;
        moved = true;

        Vector3Int characterTile = PlayerManager.singleton.Player.currentTile;
        List<Vector3Int> targetTiles = new List<Vector3Int>();
        List<Vector3Int> moveableTiles = new List<Vector3Int> { currentTile };
        MonsterInfo.MonsterPattern pattern = info.patterns[0];

        switch (pattern.pattern)
        {
            case MonsterPatternType.Basic:
            case MonsterPatternType.Range:
                moveableTiles.AddRange(Arena.singleton.getPosList(AreaShape.Hexagon, pattern.moveRange, currentTile));
                break;
        }

        moveableTiles = moveableTiles.FindAll(tile => (
            tile != characterTile &&
            Arena.singleton.tilemap.GetTile(tile) != null
        ));

        while (moveableTiles.Count != 0)
        {
            // choose target tiles to move
            switch (pattern.pattern)
            {
                case MonsterPatternType.Basic:
                    targetTiles = ShortenDistance(moveableTiles, characterTile);
                    break;
                case MonsterPatternType.Range:
                    targetTiles = StayDistance(pattern.attackRange, moveableTiles, characterTile, true);
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
        stuned = false;
        attackDirection = CalAttackDirection();

        animator.SetBool("Stuned", false);
    }

    private int CalAttackDirection()
    {
        Vector3Int characterTile = PlayerManager.singleton.Player.currentTile;
        List<int> directionList = new List<int>();
        MonsterInfo.MonsterPattern pattern = info.patterns[0];

        switch (pattern.pattern)
        {
            case MonsterPatternType.Range:
                for (int i = 0; i < 6; i++)
                {
                    List<Vector3Int> attackableArea = Arena.singleton.getPosListDirection(pattern.attackRange, currentTile, i);
                    if (attackableArea.Contains(characterTile)) return i;
                    for (int j = 1; j < pattern.attackRange; j++)
                    {
                        if (Arena.singleton.getPosListNear(attackableArea[j]).Contains(characterTile))
                        {
                            directionList.Add(i);
                        }
                    }
                }
                break;
        }

        if (directionList.Count == 0) return -1;
        return directionList[Random.Range(0, directionList.Count)];
    }

    public bool Attack()
    {
        Vector3Int characterTile = PlayerManager.singleton.Player.currentTile;
        if (AttackArea().Contains(characterTile))
        {
            PlayerManager.singleton.TakeDamage(info.patterns[0].damage);
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
            yield return new WaitForSeconds(Time.deltaTime);
            radiant2 -= Mathf.PI * 5 * Time.deltaTime;
        }
        radiant2 = 0f;
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

    private List<Vector3Int> StayDistance(int idealDistance, List<Vector3Int> moveableTiles, Vector3Int characterTile, bool straightLine = false)
    {
        List<Vector3Int> targetTiles = new List<Vector3Int>();
        int minDistance = int.MaxValue;

        foreach (Vector3Int tile in moveableTiles)
        {
            int distance = Mathf.Abs(CalDistance(tile, characterTile) - idealDistance);

            if (straightLine && Arena.singleton.getPosList(AreaShape.Line, idealDistance, tile).Contains(characterTile))
            {
                distance -= 99;
            }

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
