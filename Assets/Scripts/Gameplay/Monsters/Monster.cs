using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Monster : MoveableSprite
{
    public MonsterInfo info;
    public GameObject healthBar;
    public GameObject healthText;
    public bool moved;

    private int healthAmount;
    private Vector3 healthLocalScale;
    private float healthBarSize;

    protected override void Start()
    {
        base.Start();

        animator.runtimeAnimatorController = info.animatorController;

        healthAmount = info.maxHealth;
        healthLocalScale = healthBar.transform.localScale;
        healthBarSize = healthLocalScale.x;
    }

    protected override void Update()
    {
        base.Update();

        healthLocalScale.x = (float)healthAmount / (float)info.maxHealth * healthBarSize;
        healthBar.transform.localScale = healthLocalScale;
        healthText.GetComponent<TMPro.TextMeshProUGUI>().text = healthAmount.ToString();
    }

    public void Move()
    {
        if (moved) return;
        moved = true;

        Vector3Int characterTile = Arena.singleton.mCharacter.GetComponent<MoveableSprite>().currentTile;
        List<Vector3Int> targetTiles = new List<Vector3Int>();
        List<Vector3Int> moveableTiles = new List<Vector3Int> { currentTile };

        switch(info.pattern)
        {
            case MonsterPattern.Basic:
                moveableTiles.AddRange(Arena.singleton.getPosListNear(currentTile));
                moveableTiles = moveableTiles.FindAll(tile => tile != characterTile);
                break;
        }

        while(moveableTiles.Count != 0)
        {
            switch(info.pattern)
            {
                case MonsterPattern.Basic:
                    int minDistance = int.MaxValue;

                    foreach(Vector3Int tile in moveableTiles)
                    {
                        int distance = CalDistance(tile, characterTile);
                        if(distance < minDistance)
                        {
                            minDistance = distance;
                            targetTiles = new List<Vector3Int> { tile };
                        }
                        else if (distance == minDistance) targetTiles.Add(tile);
                    }
                    break;
            }

            foreach(Vector3Int tile in targetTiles)
            {
                moveableTiles.Remove(tile);

                Monster nearbyMonster = MonsterManager.singleton.FindMonsterByTile(tile);
                if (nearbyMonster != null) nearbyMonster.Move();
            }

            targetTiles = targetTiles.FindAll(tile => (
                tile == currentTile ||
                MonsterManager.singleton.FindMonsterByTile(tile) == null
            ));

            if (targetTiles.Count == 0) continue;

            Vector3Int destination = targetTiles[Random.Range(0, targetTiles.Count)];
            SetMovement(destination);
            break;
        }
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

    private int CalDistance(Vector3Int tile1, Vector3Int tile2)
    {
        float x1 = (float)tile1.x + 0.5f * (Mathf.Abs(tile1.y) % 2);
        float x2 = (float)tile2.x + 0.5f * (Mathf.Abs(tile2.y) % 2);
        float dy = Mathf.Abs((float)tile1.y - (float)tile2.y);
        float dx = Mathf.Abs(x1 - x2);
        int distance = (int) (dy + Mathf.Max(dx - dy/2, 0));
        return distance;
    }
}
