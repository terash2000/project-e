using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MoveableSprite
{
    public MonsterInfo info;
    public int currentMove = 0;
    public bool moved;
    public bool attacked;
    private bool stuned;
    private Dictionary<Status, int> statusDict = new Dictionary<Status, int>();
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject healthText;
    [SerializeField] private GameObject damageText;
    [SerializeField] private SpriteRenderer damageIcon;
    [SerializeField] private GameObject previewDamage;
    [SerializeField] private HorizontalLayoutGroup statusContainer;
    [SerializeField] private GameObject statusPrefab;
    private DamagePopup damagePopup;
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
            Vector3 characterPos = grid.CellToWorld(PlayerManager.Instance.Player.currentTile);
            Vector3 direction = characterPos - transform.position;
            direction.Normalize();
            transform.position = transform.position + 0.25f * Mathf.Sin(radiant2) * direction;
            lookDirection = direction;
        }
        else animator.SetBool("Attacking", false);

        if (!IsMoving() && attackDirection != -1)
        {
            lookDirection = Arena.Instance.GetDirectionVector(attackDirection);
        }

        healthLocalScale.x = (float)healthAmount / (float)info.maxHealth * healthBarSize;
        healthBar.transform.localScale = healthLocalScale;
        healthText.GetComponent<TMPro.TextMeshProUGUI>().text = healthAmount.ToString();

        int damage = stuned ? 0 : info.patterns[currentMove].damage;
        damageText.GetComponent<TMPro.TextMeshProUGUI>().text = damage.ToString();

        // preview damage
        if (Arena.Instance.TargetPosList.Contains(currentTile) &&
            CardController.selected &&
            Arena.Instance.SelectedCard.GetDamage() > 0)
        {
            previewDamage.SetActive(true);
            string cardDamage = Arena.Instance.SelectedCard.GetDamage().ToString();
            previewDamage.GetComponent<TMPro.TextMeshProUGUI>().text = cardDamage;
        }
        else previewDamage.SetActive(false);
    }

    public int TakeDamage(int damage)
    {
        GameObject dp = Instantiate(previewDamage, GetComponentInChildren<Canvas>().transform);
        dp.SetActive(true);
        dp.GetComponent<TMPro.TextMeshProUGUI>().text = damage.ToString();
        damagePopup = dp.AddComponent<DamagePopup>();

        healthAmount -= damage;
        if (healthAmount <= 0)
        {
            Die();
            return 0;
        }
        return healthAmount;
    }

    public List<Vector3Int> AttackArea()
    {
        List<Vector3Int> area = new List<Vector3Int>();
        if (stuned) return area;

        MonsterInfo.MonsterPattern pattern = info.patterns[currentMove];
        switch (pattern.pattern)
        {
            case MonsterPatternType.Basic:
                area.AddRange(Arena.Instance.GetPosListNear(currentTile));
                break;
            case MonsterPatternType.Range:
                area.AddRange(Arena.Instance.GetPosListDirection(pattern.attackRange, currentTile, attackDirection));
                break;
        }

        return area;
    }

    public void GainStatus(Status status, int amount)
    {
        switch (status)
        {
            case Status.Stun:
                Stun();
                break;
            default:
                if (!statusDict.ContainsKey(status))
                {
                    statusDict.Add(status, amount);
                }
                else statusDict[status] += amount;

                UpdateStatusIcon();
                break;
        }
    }

    public void Stun()
    {
        RemoveHighlight();
        stuned = true;
        animator.SetBool("Stuned", true);
    }

    public bool TriggerStatus()
    {
        if (statusDict.Count == 0) return false;

        foreach (KeyValuePair<Status, int> status in statusDict)
        {
            switch (status.Key)
            {
                case Status.Acid:
                case Status.Burn:
                    TakeDamage(status.Value);
                    break;
            }
        }

        statusDict = statusDict.Where(i => i.Value > 1)
            .ToDictionary(i => i.Key, i => i.Value - 1);

        UpdateStatusIcon();
        return true;
    }

    public void Move()
    {
        if (moved || stuned) return;
        moved = true;

        currentMove = (currentMove + 1) % info.patterns.Count;

        Vector3Int characterTile = PlayerManager.Instance.Player.currentTile;
        List<Vector3Int> targetTiles = new List<Vector3Int>();
        List<Vector3Int> moveableTiles = new List<Vector3Int> { currentTile };
        MonsterInfo.MonsterPattern pattern = info.patterns[currentMove];

        switch (pattern.pattern)
        {
            case MonsterPatternType.Basic:
            case MonsterPatternType.Range:
                moveableTiles.AddRange(Arena.Instance.GetPosList(AreaShape.Hexagon, pattern.moveRange, currentTile));
                break;
        }

        moveableTiles = moveableTiles.FindAll(tile => (
            tile != characterTile &&
            Arena.Instance.tilemap.GetTile(tile) != null
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

                Monster nearbyMonster = MonsterManager.Instance.FindMonsterByTile(tile);
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
        SetAttackIcon(info.patterns[currentMove].pattern);

        animator.SetBool("Stuned", false);
    }

    private int CalAttackDirection()
    {
        Vector3Int characterTile = PlayerManager.Instance.Player.currentTile;
        List<int> directionList = new List<int>();
        MonsterInfo.MonsterPattern pattern = info.patterns[currentMove];

        switch (pattern.pattern)
        {
            case MonsterPatternType.Range:
                for (int i = 0; i < 6; i++)
                {
                    List<Vector3Int> attackableArea = Arena.Instance.GetPosListDirection(pattern.attackRange, currentTile, i);
                    if (attackableArea.Contains(characterTile)) return i;
                    for (int j = 1; j < pattern.attackRange; j++)
                    {
                        if (Arena.Instance.GetPosListNear(attackableArea[j]).Contains(characterTile))
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
        Vector3Int characterTile = PlayerManager.Instance.Player.currentTile;
        if (AttackArea().Contains(characterTile))
        {
            PlayerManager.Instance.TakeDamage(info.patterns[currentMove].damage);
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
        return info.patterns[currentMove].pattern != MonsterPatternType.Basic;
    }

    private void Die()
    {
        damagePopup.transform.SetParent(Arena.Instance.transform);
        MonsterManager.Instance.monsters.Remove(this);
        RemoveHighlight();
        Destroy(gameObject);
    }

    private void RemoveHighlight()
    {
        if (ShowAttackArea())
        {
            Arena.Instance.RemoveMonsterHighlight(AttackArea());
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

            if (straightLine && Arena.Instance.GetPosList(AreaShape.Line, idealDistance, tile).Contains(characterTile))
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
            MonsterManager.Instance.FindMonsterByTile(tile) == null
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

    private void SetAttackIcon(MonsterPatternType pattern)
    {
        switch (pattern)
        {
            case MonsterPatternType.Basic:
                damageIcon.sprite = MonsterManager.Instance.SwordIcon;
                break;
            case MonsterPatternType.Range:
                damageIcon.sprite = MonsterManager.Instance.BowIcon;
                break;
        }
    }

    private void UpdateStatusIcon()
    {
        for (int i = 0; i < statusContainer.transform.childCount; i++)
        {
            Destroy(statusContainer.transform.GetChild(i).gameObject);
        }

        foreach (KeyValuePair<Status, int> status in statusDict)
        {
            GameObject statusObj = Instantiate(statusPrefab, statusContainer.transform);
            statusObj.GetComponent<MonsterStatus>().Init(status.Key, status.Value);
        }
    }
}
