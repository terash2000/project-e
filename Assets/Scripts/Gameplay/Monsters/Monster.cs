using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Monster : GameCharacter
{
    private const float ATTACK_ANIMATION_RANGE = 0.4f;
    public const float DAMAGE_COOLDOWN_TIME = 0.2f;

    public MonsterInfo info;
    public int currentMove = 0;
    public bool moved;
    public bool attacked;

    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject healthText;
    [SerializeField] private GameObject damageText;
    [SerializeField] private SpriteRenderer damageIcon;
    [SerializeField] private GameObject previewDamage;
    [SerializeField] private HorizontalLayoutGroup statusContainer;
    [SerializeField] private GameObject statusPrefab;
    [SerializeField] private Color damageColor;
    private int healthAmount;
    private Vector3 healthLocalScale;
    private float healthBarSize;
    private bool stuned;
    private int attackDirection = -1;
    private float radiant2 = 0f;
    private float damagePopupCooldown = 0f;
    private Queue<KeyValuePair<int, Color>> damageQueue = new Queue<KeyValuePair<int, Color>>();

    public int HealthAmount
    {
        get { return healthAmount; }
        set { healthAmount = value; }
    }

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
            transform.position = transform.position + ATTACK_ANIMATION_RANGE * Mathf.Sin(radiant2) * direction;
            lookDirection = direction;
        }
        else animator.SetBool("Attacking", false);

        if (!IsMoving() && attackDirection != -1)
        {
            lookDirection = Arena.Instance.GetDirectionVector(attackDirection);
        }

        // hp
        healthLocalScale.x = (float)healthAmount / (float)info.maxHealth * healthBarSize;
        healthBar.transform.localScale = healthLocalScale;
        healthText.GetComponent<TextMeshProUGUI>().text = healthAmount.ToString();

        // attack damage
        if (stuned)
        {
            damageText.SetActive(false);
        }
        else
        {
            damageText.SetActive(true);
            int damage = info.patterns[currentMove].damage;
            damageText.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        }

        // preview damage
        if (Arena.Instance.TargetPosList.Contains(currentTile) &&
            CardController.selected &&
            Arena.Instance.SelectedCard.GetDamage() > 0)
        {
            previewDamage.SetActive(true);
            string cardDamage = Arena.Instance.SelectedCard.GetDamage().ToString();
            previewDamage.GetComponent<TextMeshProUGUI>().text = cardDamage;
        }
        else previewDamage.SetActive(false);

        // damage popup
        if (damagePopupCooldown > 0)
        {
            damagePopupCooldown -= Time.deltaTime;
        }
        else if (damageQueue.Count != 0)
        {
            CreateDamagePopup(damageQueue.Dequeue());
        }
    }

    public override int TakeDamage(int damage, Color? color = null)
    {
        KeyValuePair<int, Color> damagePair = new KeyValuePair<int, Color>(damage, color ?? damageColor);
        CreateDamagePopup(damagePair);

        // Prevent Die() from being executed twice when the monster has more than one status effect
        if (healthAmount == 0)
            return 0;

        healthAmount -= damage;
        if (healthAmount <= 0)
        {
            healthAmount = 0;
            StartCoroutine(Die());
        }
        return healthAmount;
    }

    protected override void Stun()
    {
        stuned = true;
        animator.SetBool("Stuned", true);
        RemoveHighlight();
        SetAttackIcon();
    }

    protected override void UpdateStatusIcon()
    {
        for (int i = 0; i < statusContainer.transform.childCount; i++)
        {
            Destroy(statusContainer.transform.GetChild(i).gameObject);
        }

        foreach (KeyValuePair<Status, int> status in _statusDict)
        {
            GameObject statusObj = Instantiate(statusPrefab, statusContainer.transform);
            statusObj.GetComponent<StatusDisplay>().Init(status.Key, status.Value);
        }
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
        SetAttackIcon();

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
            PlayerManager.Instance.Player.TakeDamage(info.patterns[currentMove].damage);
            foreach (Status status in info.patterns[currentMove].attackStatusEffect.Keys)
            {
                int strength = info.patterns[currentMove].attackStatusEffect[status];
                PlayerManager.Instance.Player.GainStatus(status, strength);
            }
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

    private IEnumerator Die()
    {
        MonsterManager.Instance.monsters.Remove(this);
        RemoveHighlight();
        do
        {
            yield return new WaitForEndOfFrame();
        }
        while (damageQueue.Count > 0);

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

    private void SetAttackIcon()
    {
        if (stuned)
        {
            damageIcon.sprite = MonsterManager.Instance.StunIcon;
            return;
        }
        switch (info.patterns[currentMove].pattern)
        {
            case MonsterPatternType.Basic:
                damageIcon.sprite = MonsterManager.Instance.SwordIcon;
                break;
            case MonsterPatternType.Range:
                damageIcon.sprite = MonsterManager.Instance.BowIcon;
                break;
        }
    }

    private void CreateDamagePopup(KeyValuePair<int, Color> damage)
    {
        if (damagePopupCooldown > 0)
        {
            damageQueue.Enqueue(damage);
        }
        else
        {
            GameObject damagePopup = Instantiate(previewDamage, GetComponentInChildren<Canvas>().transform);
            damagePopup.SetActive(true);

            TextMeshProUGUI damagePopupText = damagePopup.GetComponent<TextMeshProUGUI>();
            damagePopupText.text = damage.Key.ToString();
            damagePopupText.color = damage.Value;

            damagePopup.AddComponent<DamagePopup>();
            damagePopup.transform.SetParent(Arena.Instance.transform);
        }

        damagePopupCooldown = DAMAGE_COOLDOWN_TIME;
    }
}
