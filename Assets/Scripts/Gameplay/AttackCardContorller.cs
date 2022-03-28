using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// sample card for demo
public class AttackCardContorller : CardController
{
    void Start()
    {
        damage = 4;
    }

    public override bool onUse(Vector3Int mousePos)
    {
        Tile tile = (Tile)Arena.singleton.tilemap.GetTile(mousePos);
        if (tile == null) return false;
        bool success = false;
        foreach (Vector3Int pos in Arena.singleton.TargetPosList)
        {
            Monster monster = MonsterManager.singleton.FindMonsterByTile(pos);
            if (monster != null)
            {
                monster.TakeDamage(damage);
                // monster.Stun();
                success = true;
            }
        }
        return success;

    }
}
