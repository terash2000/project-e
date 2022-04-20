using System;
using UnityEngine;

[Serializable]
public class StoryAction
{
    public ActionType Type;
    public int Amount;
    public string Name;
    public StoryEvent rootEvent;

    public enum ActionType
    {
        ChangeGold,
        CompleteStory,
        AddCardToDeck,
        ChangeHP,
        ChangeMaxHP,
        UnlockCard,
        ChooseCardToRemove
    }

    public void Trigger()
    {
        switch (Type)
        {
            case ActionType.ChangeGold:
                PlayerData.Gold += Amount;
                break;

            case ActionType.CompleteStory:
                if (StoryMenu.CompletedStory.Contains(rootEvent.name)) break;

                StoryMenu.CompletedStory.Add(rootEvent.name);
                SaveSystem.Save();
                break;

            case ActionType.AddCardToDeck:
                // add the card to deck
                InGameCard card;
                if (Name != "")
                {
                    card = new InGameCard(CardCollection.Instance.FindCardByName(Name));
                }
                else card = new InGameCard(CardCollection.Instance.RandomCard());

                for (int i = 0; i < Amount; i++)
                {
                    PlayerData.Deck.Add(card);
                }

                string header = "Acquire";
                if (Amount > 1) header += " x" + Amount.ToString();
                DialogManager.Instance.ShowPopup(header, card.BaseCard.CardName);
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

            case ActionType.UnlockCard:
                CardCollection.UnlockCard(Name);
                DialogManager.Instance.ShowPopup("Unlock!", Name);
                SaveSystem.SaveUnlockData();
                break;

            case ActionType.ChooseCardToRemove:
                if (PlayerData.Deck.Count == 0) break;
                DialogManager.Instance.OpenRemoveCardMenu();
                break;
        }
    }
}
