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

    public override bool OnUse(Vector3Int mousePos)
    {
        Tile tile = (Tile)Arena.Instance.tilemap.GetTile(mousePos);
        if (tile == null) return false;
        bool success = false;
        foreach (Vector3Int pos in Arena.Instance.TargetPosList)
        {
            Monster monster = MonsterManager.Instance.FindMonsterByTile(pos);
            if (monster != null)
            {
                monster.TakeDamage(damage);
                // monster.Stun();
                monster.GainStatus(Status.Acid, 2);
                success = true;
            }
        }
        return success;

    }
}
