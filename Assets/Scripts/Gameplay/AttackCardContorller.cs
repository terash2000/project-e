using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// sample card for demo
public class AttackCardContorller : CardController
{
    public override bool onUse(Vector3Int mousePos)
    {
        Tile tile = (Tile)arena.tilemap.GetTile(mousePos);
        Monster monster = MonsterManager.singleton.FindMonsterByTile(mousePos);
        if (tile != null && monster != null && tile.Equals(arena.mTile))
        {
            selected = false;
            selectThisCard = false;
            this.GetComponent<Image>().color = Color.white;
            arena.hideRadius(mAreaShape, mRange);
            monster.TakeDamage(4);
            return true;
        }
        return false;
    }
}
