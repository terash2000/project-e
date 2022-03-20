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
    protected Animator animator;

    protected virtual void Start()
    {
        grid = Arena.singleton.GetComponentInChildren<GridLayout>();
        animator = GetComponent<Animator>();
        oldPosition = nextPosition = grid.CellToWorld(currentTile);
    }

    protected virtual void Update()
    {
        //Debug.Log(IsMoving());
        // ============== MOVEMENT ======================
        displacement = new Vector2(nextPosition.x-oldPosition.x, nextPosition.y-oldPosition.y);
        transform.position = oldPosition + displacement * Mathf.Cos(radiant);
        movement = displacement * Mathf.Sin(radiant);
        
        if(!Mathf.Approximately(movement.x, 0.0f) || !Mathf.Approximately(movement.y, 0.0f))
        {
            lookDirection.Set(movement.x, movement.y);
            lookDirection.Normalize();
        }
        else lookDirection = new Vector2(0, -1);

        // ============== ANIMATION =======================
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", movement.magnitude);
    }

    public void SetMovement(Vector3Int tile)
    {
        if (currentTile == tile) return;

        currentTile = tile;
        Vector2 position = grid.CellToWorld(tile);
        oldPosition = transform.position;
        nextPosition = position;
        if (!IsMoving())
        {
            StartCoroutine(Move());
        }
        else radiant = Mathf.PI/2;

        Debug.Log(currentTile);
    }

    public bool IsMoving() {
        return radiant > 0f;
    }

    private IEnumerator Move()
    {
        radiant = Mathf.PI/2;
        while(radiant > 0f){
            radiant -= Mathf.PI/2 * speed * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        radiant = 0f;
    }
}
