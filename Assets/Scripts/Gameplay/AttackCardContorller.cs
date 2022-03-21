using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// sample card for demo
public class AttackCardContorller : CardController
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && selected)
        {
            Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int mousePos = arena.grid.WorldToCell(new Vector3(oriPos.x, oriPos.y, 0));
            Tile tile = (Tile)arena.tilemap.GetTile(mousePos);
            Monster monster = MonsterManager.singleton.FindMonsterByTile(mousePos);
            if (tile != null && monster != null && tile.Equals(arena.mTile))
            {
                selected = false;
                selectThisCard = false;
                this.GetComponent<Image>().color = Color.white;
                arena.hideRadius(mAreaShape, mRange);
                monster.TakeDamage(4);
            }
        }
    }
}
