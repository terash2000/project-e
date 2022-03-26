using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// sample card for demo
public class AttackCardContorller : CardController
{
    public override bool onUse(Vector3Int mousePos)
    {
        Tile tile = (Tile)Arena.singleton.tilemap.GetTile(mousePos);
        Monster monster = MonsterManager.singleton.FindMonsterByTile(mousePos);
        if (tile != null && monster != null && Arena.singleton.AreaPosList.Contains(mousePos))
        {
            Arena.singleton.hideRadius(mAreaShape, mRange);
            monster.TakeDamage(4);
            // monster.Stun();
            return true;
        }
        return false;
    }
}
