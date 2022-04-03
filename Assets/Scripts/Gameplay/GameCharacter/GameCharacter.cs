using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
    public Vector3Int currentTile;

    [SerializeField] protected float speed;
    protected GridLayout grid;
    protected Vector2 displacement;
    protected Vector2 movement;
    protected Vector2 oldPosition;
    protected Vector2 nextPosition;
    protected Vector2 lookDirection = new Vector2(0, -1);
    protected float radiant = 0f;
    protected GameObject sprite;
    protected Animator animator;

    protected Dictionary<Status.Type, int> _statusDict = new Dictionary<Status.Type, int>();
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
            sprite = spriteTransform.gameObject;
            animator = sprite.GetComponent<Animator>();
        }
        else animator = GetComponent<Animator>();

        grid = Arena.Instance.GetComponentInChildren<GridLayout>();
        transform.position = oldPosition = nextPosition = grid.CellToWorld(currentTile);
    }

    protected virtual void Update()
    {
        // ============== MOVEMENT ======================
        displacement = new Vector2(nextPosition.x - oldPosition.x, nextPosition.y - oldPosition.y);
        transform.position = oldPosition + displacement * Mathf.Cos(radiant);
        movement = displacement * Mathf.Sin(radiant);

        if (!Mathf.Approximately(movement.x, 0.0f) || !Mathf.Approximately(movement.y, 0.0f))
        {
            lookDirection.Set(movement.x, movement.y);
            lookDirection.Normalize();
        }

        // ============== ANIMATION =======================
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", movement.magnitude);
    }

    public void SetMovement(Vector3Int tile)
    {
        if (currentTile == tile) return;
        Vector2 position = grid.CellToWorld(tile);
        oldPosition = grid.CellToWorld(currentTile);
        nextPosition = position;
        currentTile = tile;
        if (!IsMoving())
        {
            StartCoroutine(Move());
        }
        else radiant = Mathf.PI / 2;
    }

    public bool IsMoving()
    {
        return radiant > 0f;
    }

    private IEnumerator Move()
    {
        radiant = Mathf.PI / 2;
        while (radiant > 0f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            radiant -= Mathf.PI / 2 * speed * Time.deltaTime;
        }
        radiant = 0f;
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

    public virtual int TakeDamage(int damage, Status.Type? damageStatusEffect = null)
    {
        return 0;
    }

    protected virtual void Stun() { }

    protected virtual void UpdateStatusIcon() { }
}
