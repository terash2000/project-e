using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MoveableSprite
{
    private Dictionary<Status, int> statusDict = new Dictionary<Status, int>();

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void TakeDamage(int damage)
    {
        PlayerData.Health -= damage;
    }

    public void GainStatus(Status status, int amount = 1)
    {
        switch (status)
        {
            case Status.Stun:
                Stun();
                break;
            default:
                if (!statusDict.ContainsKey(status))
                {
                    statusDict.Add(status, amount);
                }
                else statusDict[status] += amount;

                break;
        }
    }

    public void Stun()
    {
        /*
        stuned = true;
        animator.SetBool("Stuned", true);
        */
    }

    public bool TriggerStatus()
    {
        if (statusDict.Count == 0) return false;

        foreach (KeyValuePair<Status, int> status in statusDict)
        {
            switch (status.Key)
            {
                case Status.Acid:
                case Status.Burn:
                    TakeDamage(status.Value);
                    break;
            }
        }

        statusDict = statusDict.Where(i => i.Value > 1)
            .ToDictionary(i => i.Key, i => i.Value - 1);

        return true;
    }
}
