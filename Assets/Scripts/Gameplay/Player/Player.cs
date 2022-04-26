using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : GameCharacter
{
    private Vector3 _localScale = new Vector3(0.12f, 0.12f, 1f);

    protected override void Start()
    {
        base.Start();
        transform.localScale = _localScale;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override int TakeDamage(int damage, Status.Type? damageStatusEffect = null)
    {
        int blockedAmount = 0;
        if (Block != 0)
        {
            if (damageStatusEffect == Status.Type.Acid)
            {
                // Acid attack will deal "Status.ACID_TO_BLOCK_MULTIPLIER" of damage to block but cannot carry over to health
                int damageToBlock = System.Convert.ToInt32(System.Math.Floor(damage * Status.ACID_TO_BLOCK_MULTIPLIER));
                blockedAmount = Block - damageToBlock < 0 ? Block : damageToBlock;
                damage = 0;
            }
            else
            {
                // Normal scenario, if the attack damage is more than the block, it'll get carried over to the health
                blockedAmount = Block - damage < 0 ? Block : damage;
                damage = damage - blockedAmount;
            }
            // Debug.Log("Attack damage of type " + damageStatusEffect + " has been blocked by " + blockedAmount + " and get carried over by " + damage);
            Block -= blockedAmount;
        }

        PlayerData.Health -= damage;
        return PlayerData.Health;
    }
}
