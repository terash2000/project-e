using System;

[Serializable]
public class StoryAction
{
    public ActionType Type;
    public int Amount;
    public string Name;
    public StoryEvent rootEvent;

    public enum ActionType
    {
        None,
        CompleteStory,
        AddCardToDeck,
        ChangeHP,
        ChangeMaxHP
    }

    public void Trigger()
    {
        switch (Type)
        {
            case ActionType.CompleteStory:
                if (StoryMenu.CompletedStory.Contains(rootEvent.name)) break;

                StoryMenu.CompletedStory.Add(rootEvent.name);

                if (Name != "")
                {
                    // unlock card
                    CardCollection.UnlockCard(Name);
                    DialogManager.Instance.ShowPopup("Unlock!", Name);
                }

                SaveSystem.SaveUnlockData();
                SaveSystem.Save();
                break;
            case ActionType.AddCardToDeck:
                // add the card to deck
                Card card = CardCollection.Instance.FindCardByName(Name);
                PlayerData.Deck.Add(card);

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
