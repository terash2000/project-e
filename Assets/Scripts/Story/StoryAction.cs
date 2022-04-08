using System;

[Serializable]
public class StoryAction
{
    public ActionType Type;
    public int Amount;
    public string Name;

    public enum ActionType
    {
        None,
        UnlockCard,
        AddCardToDeck,
        ChangeHP,
        ChangeMaxHP
    }

    public void Trigger()
    {
        switch (Type)
        {
            case ActionType.UnlockCard:
                CardCollection.Instance.UnlockCard(Name);
                DialogManager.Instance.ShowPopup("Unlock!", Name);
                break;
            case ActionType.AddCardToDeck:
                // TODO add the card to deck

                string header = "Acquire";
                if (Amount > 1) header += " x" + Amount.ToString();
                DialogManager.Instance.ShowPopup(header, Name);
                break;
            case ActionType.ChangeHP:
                PlayerData.Health += Amount;
                // player can't die in event
                if (PlayerData.Health <= 0) PlayerData.Health = 1;
                break;
            case ActionType.ChangeMaxHP:
                PlayerData.MaxHealth += Amount;
                // player can't die in event
                if (PlayerData.MaxHealth <= 0) PlayerData.MaxHealth = 1;
                break;
        }
    }
}
