using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
    [SerializeField]
    protected float Speed;

    public Vector3Int CurrentTile;

    protected GridLayout Grid;
    protected Vector2 Displacement;
    protected Vector2 Movement;
    protected Vector2 OldPosition;
    protected Vector2 NextPosition;
    protected Vector2 LookDirection = new Vector2(0, -1);
    protected float Radiant = 0f;
    protected GameObject Sprite;
    protected Animator Animator;

    private int _health;
    private int _block;
    private Dictionary<Status.Type, int> _statusDict = new Dictionary<Status.Type, int>();

    public int Health
    {
        get { return _health; }
        set { _health = value; }
    }
    public int Block
    {
        get { return _block; }
        set { _block = value; }
    }
    public Dictionary<Status.Type, int> StatusDict
    {
        get { return _statusDict; }
        set { }
    }

    protected virtual void Start()
    {
        Transform spriteTransform = transform.Find("Sprite");
        if (spriteTransform)
        {
            Sprite = spriteTransform.gameObject;
            Animator = Sprite.GetComponent<Animator>();
        }
        else Animator = GetComponent<Animator>();

        Grid = Arena.Instance.GetComponentInChildren<GridLayout>();
        transform.position = OldPosition = NextPosition = Grid.CellToWorld(CurrentTile);
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

    public bool TriggerStatus()
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
        return 0;
    }

    protected virtual void Stun() { }

    protected virtual void UpdateStatusIcon() { }
}
