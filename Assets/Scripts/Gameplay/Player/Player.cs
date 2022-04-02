using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : GameCharacter
{
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override int TakeDamage(int damage, Color? color = null)
    {
        PlayerData.Health -= damage;
        return PlayerData.Health;
    }
}
