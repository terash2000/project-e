using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Monster : GameCharacter
{
    public const float DAMAGE_COOLDOWN_TIME = 0.2f;
    private const float ATTACK_ANIMATION_RANGE = 0.4f;
    private const int PREVIEW_ATTACK_COUNT_SIZE_PERCENT = 75;

    [SerializeField]
    private GameObject _healthBar;
    [SerializeField]
    private GameObject _blockBar;
    [SerializeField]
    private GameObject _healthText;
    [SerializeField]
    private GameObject _damageText;
    [SerializeField]
    private GameObject _blockText;
    [SerializeField]
    private SpriteRenderer _damageIcon;
    [SerializeField]
    private GameObject _previewDamage;
    [SerializeField]
    private GameObject _deathIcon;
    [SerializeField]
    private HorizontalLayoutGroup _statusContainer;
    [SerializeField]
    private GameObject _statusPrefab;
    [SerializeField]
    private Color _damageColor;

    private MonsterInfo _info;
    private int _currentMove = 0;
    private bool _hasMoved;
    private bool _hasAttacked;
    private bool _isStuned;
    private Vector3 _healthLocalScale;
    private float _healthBarSize;
    private int _attackDirection = -1;
    private float _radiant2 = 0f;
    private float _damagePopupCooldown = 0f;
    private Queue<DamageQueueData> _damageQueue = new Queue<DamageQueueData>();

    public MonsterInfo Info
    {
        get { return _info; }
        set { _info = value; }
    }
    public int CurrentMove
    {
        get { return _currentMove; }
        set { _currentMove = value; }
    }
    public bool HasMoved
    {
        get { return _hasMoved; }
        set { _hasMoved = value; }
    }
    public bool HasAttacked
    {
        get { return _hasAttacked; }
        set { _hasAttacked = value; }
    }
    public bool IsStunned
    {
        get { return _isStuned; }
        set { _isStuned = value; }
    }

    protected override void Start()
    {
        base.Start();

        Animator.runtimeAnimatorController = _info.AnimatorController;
        Sprite.GetComponent<SpriteRenderer>().color = _info.SpriteColor;
        Sprite.transform.localScale = new Vector3(_info.SpriteScale, _info.SpriteScale, transform.localScale.z);

        // look at center of areana
        Vector3 direction = -transform.position;
        direction.Normalize();
        LookDirection = direction;

        Health = _info.MaxHealth;
        _healthLocalScale = _healthBar.transform.localScale;
        _healthBarSize = _healthLocalScale.x;
        Block = _info.InitialBlock;
    }

    protected override void Update()
    {
        base.Update();

        // attack animation
        if (_radiant2 != 0f)
        {
            Animator.SetBool("Attacking", true);
            Vector3 characterPos = Grid.CellToWorld(PlayerManager.Instance.Player.CurrentTile);
            Vector3 direction = characterPos - transform.position;
            direction.Normalize();
            transform.position = transform.position + ATTACK_ANIMATION_RANGE * Mathf.Sin(_radiant2) * direction;
            LookDirection = direction;
        }
        else Animator.SetBool("Attacking", false);

        if (!IsMoving() && _attackDirection != -1)
        {
            LookDirection = Arena.Instance.GetDirectionVector(_attackDirection);
        }

        // hp
        _healthLocalScale.x = _healthBarSize * (float)Health / (float)_info.MaxHealth;
        _healthBar.transform.localScale = _healthLocalScale;
        _healthText.GetComponent<TextMeshProUGUI>().text = Health.ToString();

        _blockText.GetComponent<TextMeshProUGUI>().text = Block.ToString();
        _blockText.SetActive(Block > 0);
        _blockBar.SetActive(Block > 0);

        // attack damage
        if (_isStuned)
        {
            _damageText.SetActive(false);
        }
        else
        {
            _damageText.SetActive(true);
            int damage = _info.Patterns[_currentMove].Damage;
            _damageText.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        }

        // preview damage
        if (Arena.Instance.TargetPosList.Contains(CurrentTile) &&
            CardManager.Instance.IsSelectingCard() &&
            CardManager.Instance.SelectingCard.Card.BaseCard.Type == CardType.Attack)
        {
            InGameCard selectingCard = CardManager.Instance.SelectingCard.Card;
            int cardDamage = selectingCard.GetRealDamage();
            int attackCount = selectingCard.BaseCard.Effects.FindAll(effect => effect == CardEffect.RepeatAttack).Count + 1;
            TextMeshProUGUI previewText = _previewDamage.GetComponent<TextMeshProUGUI>();
            _previewDamage.SetActive(true);
            _deathIcon.SetActive(false);
            previewText.text = cardDamage.ToString();
            if (attackCount > 1)
                previewText.text += $"<size={PREVIEW_ATTACK_COUNT_SIZE_PERCENT}%>x{attackCount}</size>";

            _deathIcon.SetActive(cardDamage * attackCount >= Health + Block);
        }
        else
        {
            _previewDamage.SetActive(false);
            _deathIcon.SetActive(false);
        }

        // damage popup
        if (_damagePopupCooldown > 0)
        {
            _damagePopupCooldown -= Time.deltaTime;
        }
        else if (_damageQueue.Count != 0)
        {
            CreateDamagePopup(_damageQueue.Dequeue());
        }
    }

    public override int TakeDamage(int damage, Status.Type? damageStatusEffect = null)
    {
        // Prevent Die() from being executed twice when the monster has more than one status effect
        if (Health == 0)
            return 0;

        Color color;
        if (damageStatusEffect == Status.Type.Acid)
            color = GameManager.Instance.AcidColor;
        else if (damageStatusEffect == Status.Type.Burn)
            color = GameManager.Instance.BurnColor;
        else
            color = _damageColor;

        int blockedAmount = 0;
        if (Block != 0)
        {
            if (damageStatusEffect == Status.Type.Acid)
            {
                // Acid attack will deal "Status.ACID_TO_BLOCK_MULTIPLIER" of damage to block but cannot carry over to health
                int damageToBlock = System.Convert.ToInt32(System.Math.Floor(damage * Status.ACID_TO_BLOCK_MULTIPLIER));
                blockedAmount = Block - damageToBlock < 0 ? Block : damageToBlock;
                damage = 0;
            }
            else
            {
                // Normal scenario, if the attack damage is more than the block, it'll get carried over to the health
                blockedAmount = Block - damage < 0 ? Block : damage;
                damage = damage - blockedAmount;
            }
            // Debug.Log("Attack damage of type " + damageStatusEffect + " has been blocked by " + blockedAmount + " and get carried over by " + damage);
            Block -= blockedAmount;
        }

        if (damage != 0 || blockedAmount != 0)
        {
            CreateDamagePopup(new DamageQueueData(damage, blockedAmount, color));
        }

        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            StartCoroutine(Die());
        }
        return Health;
    }

    protected override void Stun()
    {
        RemoveHighlight();
        _isStuned = true;
        Animator.SetBool("Stuned", true);
        SetAttackIcon();
    }

    protected override void UpdateStatusIcon()
    {
        for (int i = 0; i < _statusContainer.transform.childCount; i++)
        {
            Destroy(_statusContainer.transform.GetChild(i).gameObject);
        }

        foreach (KeyValuePair<Status.Type, int> status in StatusDict)
        {
            GameObject statusObj = Instantiate(_statusPrefab, _statusContainer.transform);
            statusObj.GetComponent<StatusDisplay>().Init(status.Key, status.Value);
        }
    }

    public List<Vector3Int> AttackArea()
    {
        List<Vector3Int> area = new List<Vector3Int>();
        if (_isStuned) return area;

        MonsterInfo.MonsterPattern pattern = _info.Patterns[_currentMove];
        switch (pattern.Pattern)
        {
            case MonsterPatternType.Basic:
            case MonsterPatternType.AttackAndBlock:
                area.AddRange(Arena.Instance.GetPosListNear(CurrentTile));
                break;
            case MonsterPatternType.Range:
                area.AddRange(Arena.Instance.GetPosListDirection(pattern.AttackRange, CurrentTile, _attackDirection));
                break;
            case MonsterPatternType.Beam:
                area.AddRange(Arena.Instance.GetPosListBeam(pattern.AttackRange, CurrentTile, _attackDirection));
                break;
            case MonsterPatternType.Star:
                area.AddRange(Arena.Instance.GetPosList(AreaShape.Line, pattern.AttackRange, CurrentTile));
                break;
        }

        return area;
    }

    public void Move()
    {
        if (_hasMoved || _isStuned) return;
        _hasMoved = true;

        _currentMove = (_currentMove + 1) % _info.Patterns.Count;

        Vector3Int characterTile = PlayerManager.Instance.Player.CurrentTile;
        List<Vector3Int> targetTiles = new List<Vector3Int>();
        List<Vector3Int> moveableTiles = new List<Vector3Int> { CurrentTile };
        MonsterInfo.MonsterPattern pattern = _info.Patterns[_currentMove];

        moveableTiles.AddRange(Arena.Instance.GetPosList(AreaShape.Hexagon, pattern.MoveRange, CurrentTile));

        moveableTiles = moveableTiles.FindAll(tile => (
            tile != characterTile &&
            Arena.Instance.Tilemap.GetTile(tile) != null
        ));

        while (moveableTiles.Count != 0)
        {
            // choose target tiles to move
            switch (pattern.Pattern)
            {
                case MonsterPatternType.Basic:
                case MonsterPatternType.AttackAndBlock:
                case MonsterPatternType.Star:
                    targetTiles = ShortenDistance(moveableTiles, characterTile);
                    break;
                case MonsterPatternType.Range:
                    targetTiles = StayDistance(pattern.AttackRange, moveableTiles, characterTile, true);
                    break;
                case MonsterPatternType.Beam:
                    targetTiles = StayDistance(pattern.AttackRange - 1, moveableTiles, characterTile, true);
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
            if (targetTiles.Contains(CurrentTile)) break;
        }
    }

    public void Refresh()
    {
        _hasMoved = false;
        _hasAttacked = false;
        _isStuned = false;
        _attackDirection = CalAttackDirection();
        SetAttackIcon();

        Animator.SetBool("Stuned", false);
    }

    private int CalAttackDirection()
    {
        Vector3Int characterTile = PlayerManager.Instance.Player.CurrentTile;
        List<int> directionList = new List<int>();
        MonsterInfo.MonsterPattern pattern = _info.Patterns[_currentMove];

        switch (pattern.Pattern)
        {
            case MonsterPatternType.Range:
            case MonsterPatternType.Beam:
                for (int i = 0; i < 6; i++)
                {
                    if (Arena.Instance.GetPosListBeam(pattern.AttackRange + 1, CurrentTile, i).Contains(characterTile))
                    {
                        directionList.Add(i);
                    }
                }
                break;
        }

        if (directionList.Count == 0) return -1;
        return directionList[Random.Range(0, directionList.Count)];
    }

    public bool Attack()
    {
        Vector3Int characterTile = PlayerManager.Instance.Player.CurrentTile;
        if (AttackArea().Contains(characterTile))
        {
            Block += _info.Patterns[_currentMove].BlockGain;
            PlayerManager.Instance.Player.TakeDamage(_info.Patterns[_currentMove].Damage);
            foreach (Status.Type status in _info.Patterns[_currentMove].AttackStatusEffect.Keys)
            {
                int strength = _info.Patterns[_currentMove].AttackStatusEffect[status];
                PlayerManager.Instance.Player.GainStatus(status, strength);
            }
            StartCoroutine(AttackAnimation());
            _hasAttacked = true;
            return true;
        }
        return false;
    }

    private IEnumerator AttackAnimation()
    {
        _radiant2 = Mathf.PI;
        while (_radiant2 > 0f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            _radiant2 -= Mathf.PI * 5 * Time.deltaTime;
        }
        _radiant2 = 0f;
    }

    public bool CanMoveAfterAttack()
    {
        return _info.Patterns.Count > 1;
    }

    public bool ShowAttackArea()
    {
        return _info.Patterns[_currentMove].Pattern == MonsterPatternType.Range ||
            _info.Patterns[_currentMove].Pattern == MonsterPatternType.Beam ||
            _info.Patterns[_currentMove].Pattern == MonsterPatternType.Star;
    }

    private IEnumerator Die()
    {
        MonsterManager.Instance.Monsters.Remove(this);
        RemoveHighlight();
        do
        {
            yield return new WaitForEndOfFrame();
        }
        while (_damageQueue.Count > 0);

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
        if (_isStuned)
        {
            _damageIcon.sprite = MonsterManager.Instance.StunIcon;
            return;
        }
        switch (_info.Patterns[_currentMove].Pattern)
        {
            case MonsterPatternType.Basic:
            case MonsterPatternType.Star:
                _damageIcon.sprite = MonsterManager.Instance.SwordIcon;
                break;
            case MonsterPatternType.Range:
            case MonsterPatternType.Beam:
                _damageIcon.sprite = MonsterManager.Instance.BowIcon;
                break;
            case MonsterPatternType.AttackAndBlock:
                _damageIcon.sprite = MonsterManager.Instance.SwordAndShieldIcon;
                break;
        }
    }

    private void CreateDamagePopup(DamageQueueData data)
    {
        if (_damagePopupCooldown > 0)
        {
            _damageQueue.Enqueue(data);
        }
        else
        {
            Canvas monsterCanvas = GetComponentInChildren<Canvas>();

            if (data.damage != 0 || data.color == _damageColor)
            {
                GameObject damagePopup = Instantiate(_previewDamage, monsterCanvas.transform);
                damagePopup.SetActive(true);
                damagePopup.name = "Popup Damage";

                TextMeshProUGUI damagePopupText = damagePopup.GetComponent<TextMeshProUGUI>();
                damagePopupText.text = data.damage.ToString();
                damagePopupText.color = data.color;

                damagePopup.AddComponent<DamagePopup>();
                damagePopup.transform.SetParent(Arena.Instance.transform);
            }

            if (data.block != 0)
            {
                GameObject blockPopup = Instantiate(_previewDamage, monsterCanvas.transform);
                blockPopup.SetActive(true);
                blockPopup.name = "Popup Block";

                TextMeshProUGUI blockPopupText = blockPopup.GetComponent<TextMeshProUGUI>();
                blockPopupText.text = data.block.ToString();
                blockPopupText.color = data.color;

                blockPopup.AddComponent<BlockPopup>();
                blockPopup.transform.SetParent(Arena.Instance.transform);
            }
            _damagePopupCooldown = DAMAGE_COOLDOWN_TIME;
        }
    }
}
