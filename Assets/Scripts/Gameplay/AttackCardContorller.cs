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
                // monster.GainStatus(Status.Stun);
                monster.GainStatus(Status.Type.Acid, 2);
                monster.GainStatus(Status.Type.Burn, 3);
                success = true;
            }
        }
        return success;

    }
}
