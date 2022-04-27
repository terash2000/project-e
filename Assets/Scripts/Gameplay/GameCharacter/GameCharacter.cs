using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCharacter : MonoBehaviour
{
    public const float DAMAGE_COOLDOWN_TIME = 0.2f;

    public Vector3Int CurrentTile;

    [SerializeField] protected float Speed;
    [SerializeField] private HorizontalLayoutGroup _statusContainer;
    [SerializeField] private GameObject _statusPrefab;
    [SerializeField] protected GameObject _previewDamage;
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private GameObject _blockBar;
    [SerializeField] private GameObject _healthText;
    [SerializeField] private GameObject _blockText;
    [SerializeField] private Color _damageColor;

    protected GridLayout Grid;
    protected Vector2 Displacement;
    protected Vector2 Movement;
    protected Vector2 OldPosition;
    protected Vector2 NextPosition;
    protected Vector2 LookDirection = new Vector2(0, -1);
    protected float Radiant = 0f;
    protected GameObject Sprite;
    protected Animator Animator;

    private Vector3 _healthLocalScale;
    private float _healthBarSize;
    private int _block;
    private Dictionary<Status.Type, int> _statusDict = new Dictionary<Status.Type, int>();
    private float _damagePopupCooldown = 0f;
    private Queue<DamageQueueData> _damageQueue = new Queue<DamageQueueData>();

    public int Block
    {
        get { return _block; }
    }
    public Dictionary<Status.Type, int> StatusDict
    {
        get { return _statusDict; }
    }
    public Queue<DamageQueueData> DamageQueue
    {
        get { return _damageQueue; }
    }

    protected virtual void Stun() { }
    protected virtual int GetHealth() { return 0; }
    protected virtual void SetHealth(int value) { }
    protected virtual int GetMaxHealth() { return 0; }

    void Awake()
    {
        Transform spriteTransform = transform.Find("Sprite");
        if (spriteTransform)
        {
            Sprite = spriteTransform.gameObject;
            Animator = Sprite.GetComponent<Animator>();
        }
        else Animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        Grid = Arena.Instance.GetComponentInChildren<GridLayout>();
        transform.position = OldPosition = NextPosition = Grid.CellToWorld(CurrentTile);

        _healthLocalScale = _healthBar.transform.localScale;
        _healthBarSize = _healthLocalScale.x;

        UpdateHealthBar();
        UpdateBlockBar();
    }

    protected virtual void Update()
    {
        // ============== MOVEMENT ======================
        Displacement = new Vector2(NextPosition.x - OldPosition.x, NextPosition.y - OldPosition.y);
        transform.position = OldPosition + Displacement * Mathf.Cos(Radiant);
        Movement = Displacement * Mathf.Sin(Radiant);

        if (!Mathf.Approximately(Movement.x, 0.0f) || !Mathf.Approximately(Movement.y, 0.0f))
        {
            LookDirection.Set(Movement.x, Movement.y);
            LookDirection.Normalize();
        }

        // ============== ANIMATION =======================
        Animator.SetFloat("Look X", LookDirection.x);
        Animator.SetFloat("Look Y", LookDirection.y);
        Animator.SetFloat("Speed", Movement.magnitude);

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

    public void SetMovement(Vector3Int tile)
    {
        if (CurrentTile == tile) return;
        Vector2 position = Grid.CellToWorld(tile);
        OldPosition = transform.position;
        NextPosition = position;
        CurrentTile = tile;
        if (!IsMoving())
        {
            StartCoroutine(Move());
        }
        else Radiant = Mathf.PI / 2;
    }

    public bool IsMoving()
    {
        return Radiant > 0f;
    }

    private IEnumerator Move()
    {
        Radiant = Mathf.PI / 2;
        while (Radiant > 0f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            Radiant -= Mathf.PI / 2 * Speed * Time.deltaTime;
        }
        Radiant = 0f;
    }

    public virtual void GainStatus(Status.Type status, int amount = 1)
    {
        switch (status)
        {
            case Status.Type.Stun:
                Stun();
                break;
            default:
                if (!_statusDict.ContainsKey(status))
                {
                    _statusDict.Add(status, amount);
                }
                else _statusDict[status] += amount;

                UpdateStatusIcon();
                break;
        }
    }

    public virtual bool TriggerStatus()
    {
        if (_statusDict.Count == 0) return false;

        foreach (KeyValuePair<Status.Type, int> status in _statusDict)
        {
            switch (status.Key)
            {
                case Status.Type.Acid:
                    TakeDamage(status.Value, Status.Type.Acid);
                    break;
                case Status.Type.Burn:
                    TakeDamage(status.Value, Status.Type.Burn);
                    break;
            }
        }

        _statusDict = _statusDict.Where(i => i.Value > 1)
            .ToDictionary(i => i.Key, i => i.Value - 1);

        UpdateStatusIcon();
        return true;
    }

    public bool MoveDirection(int direction)
    {
        Vector3Int targetPos = Arena.Instance.GetPosDirection(CurrentTile, direction);
        if (Arena.Instance.Tilemap.GetTile(targetPos) != null
                && MonsterManager.Instance.FindMonsterByTile(targetPos) == null
                && PlayerManager.Instance.Player.CurrentTile != targetPos)
        {
            SetMovement(targetPos);
            return true;
        }
        return false;
    }

    public virtual int TakeDamage(int damage, Status.Type? damageStatusEffect = null)
    {
        // Prevent Die() from being executed twice when having more than one status effect
        if (GetHealth() == 0)
            return 0;

        int blockedAmount = 0;
        if (_block != 0)
        {
            if (damageStatusEffect == Status.Type.Acid)
            {
                // Acid attack will deal "Status.ACID_TO_BLOCK_MULTIPLIER" of damage to block but cannot carry over to health
                int damageToBlock = System.Convert.ToInt32(System.Math.Floor(damage * Status.ACID_TO_BLOCK_MULTIPLIER));
                blockedAmount = _block - damageToBlock < 0 ? _block : damageToBlock;
                damage = 0;
            }
            else
            {
                // Normal scenario, if the attack damage is more than the block, it'll get carried over to the health
                blockedAmount = _block - damage < 0 ? _block : damage;
                damage = damage - blockedAmount;
            }
            // Debug.Log("Attack damage of type " + damageStatusEffect + " has been blocked by " + blockedAmount + " and get carried over by " + damage);
            _block -= blockedAmount;
        }

        if (damage != 0 || blockedAmount != 0)
        {
            Color color;
            if (damageStatusEffect == Status.Type.Acid)
                color = GameManager.Instance.AcidColor;
            else if (damageStatusEffect == Status.Type.Burn)
                color = GameManager.Instance.BurnColor;
            else
                color = _damageColor;

            CreateDamagePopup(new DamageQueueData(damage, blockedAmount, color));
        }

        SetHealth(GetHealth() - damage);

        UpdateHealthBar();
        UpdateBlockBar();

        return GetHealth();
    }

    public void GainBlock(int amount)
    {
        _block += amount;
        UpdateBlockBar();
    }

    public void ResetBlock()
    {
        _block = 0;
        UpdateBlockBar();
    }

    protected void UpdateHealthBar()
    {
        _healthLocalScale.x = _healthBarSize * (float)GetHealth() / (float)GetMaxHealth();
        _healthBar.transform.localScale = _healthLocalScale;
        _healthText.GetComponent<TextMeshProUGUI>().text = GetHealth().ToString();
    }

    protected void UpdateBlockBar()
    {
        _blockText.GetComponent<TextMeshProUGUI>().text = _block.ToString();
        _blockText.SetActive(_block > 0);
        _blockBar.SetActive(_block > 0);
    }

    protected void UpdateStatusIcon()
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
