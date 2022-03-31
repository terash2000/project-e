using System;
using UnityEngine;

[Serializable]
public class StoryAction
{
    public ActionType type;
    public int amount;
    public string name;

    public enum ActionType
    {
        None,
        UnlockCard,
        AddCardToDeck,
        ChangeHP
    }

    public void Trigger()
    {
        switch (type)
        {
            case ActionType.UnlockCard:
                CardCollection.Instance.UnlockCard(name);
                DialogManager.Instance.ShowPopup("Unlock!", name);
                break;
            case ActionType.AddCardToDeck:
                // TODO add the card to deck

                string header = "Acquire";
                if (amount > 1) header += " x" + amount.ToString();
                DialogManager.Instance.ShowPopup(header, name);
                break;
            case ActionType.ChangeHP:
                PlayerData.Health += amount;
                // player can't die in event
                if (PlayerData.Health <= 0) PlayerData.Health = 1;
                break;
        }
    }
}
