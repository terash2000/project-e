using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public MonsterInfo info;
    public GridLayout mGrid;
    public Vector3Int currentTile;
    public GameObject healthBar;
    public GameObject healthText;

    private Animator animator;
    private int healthAmount;
    private Vector3 healthLocalScale;
    private float healthBarSize;

    // =========== MOVEMENT ==============
    [SerializeField] private float speed;
    private Vector2 displacement;
    private Vector2 movement;
    private Vector2 oldPosition;
    private Vector2 nextPosition;
    private Vector2 lookDirection = new Vector2(0, -1);
    private float radiant;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = info.animatorController;

        oldPosition = nextPosition = mGrid.CellToWorld(currentTile);
        healthAmount = info.maxHealth;

        healthLocalScale = healthBar.transform.localScale;
        healthBarSize = healthLocalScale.x;
    }

    void Update()
    {
        //if(Input.GetMouseButtonUp(0))
        //{
        //    Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Vector3Int pos = mGrid.WorldToCell(oriPos);
        //    SetMovement(pos);
        //}

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

        // ============== HEALTHBAR =======================
        healthLocalScale.x = (float)healthAmount / (float)info.maxHealth * healthBarSize;
        healthBar.transform.localScale = healthLocalScale;
        healthText.GetComponent<TMPro.TextMeshProUGUI>().text = healthAmount.ToString();
    }

    public void SetMovement(Vector3Int tile)
    {
        currentTile = tile;
        Vector2 position = mGrid.CellToWorld(tile);
        oldPosition = transform.position;
        nextPosition = position;
        radiant = Mathf.PI/2;
        if (movement.magnitude == 0f)
            StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while(radiant > 0f){
            radiant -= Mathf.PI/2 * speed * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        radiant = 0f;
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

    private void Die()
    {
        Destroy(gameObject);
    }
}
