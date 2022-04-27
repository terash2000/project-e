using System.Collections.Generic;
using UnityEngine;

public class Player : GameCharacter
{
    private Vector3 _localScale = new Vector3(0.12f, 0.12f, 1f);

    protected override void Start()
    {
        transform.localScale = _localScale;
        base.Start();
    }

    public override void GainStatus(Status.Type status, int amount = 1)
    {
        base.GainStatus(status, amount);
        if (status == Status.Type.Strong || status == Status.Type.Weak)
        {
            // rewrite card text
            Transform hand = CardManager.Instance.HandPanel.transform;
            for (int i = 0; i < hand.childCount; i++)
            {
                CardDisplay cardDisplay = hand.GetChild(i).GetComponent<CardDisplay>();
                cardDisplay.Render();
            }
        }
    }

    public override bool TriggerStatus()
    {
        if (base.TriggerStatus())
        {
            // rewrite card text
            Transform hand = CardManager.Instance.HandPanel.transform;
            for (int i = 0; i < hand.childCount; i++)
            {
                CardDisplay cardDisplay = hand.GetChild(i).GetComponent<CardDisplay>();
                cardDisplay.Render();
            }
            return true;
        }
        return false;
    }

    protected override int GetHealth()
    {
        return PlayerData.Health;
    }

    protected override void SetHealth(int value)
    {
        PlayerData.Health = value;
    }

    protected override int GetMaxHealth()
    {
        return PlayerData.MaxHealth;
    }
}
