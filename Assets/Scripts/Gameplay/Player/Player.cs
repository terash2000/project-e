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
        PlayerData.Health -= damage;
        return PlayerData.Health;
    }
}
