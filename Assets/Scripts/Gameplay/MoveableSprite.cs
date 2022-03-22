using System.Collections;
using UnityEngine;

public class MoveableSprite : MonoBehaviour
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

    protected virtual void Start()
    {
        Transform spriteTransform = transform.Find("Sprite");
        if (spriteTransform)
        {
            sprite = spriteTransform.gameObject;
            animator = sprite.GetComponent<Animator>();
        }
        else animator = GetComponent<Animator>();

        grid = Arena.singleton.GetComponentInChildren<GridLayout>();
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
}
