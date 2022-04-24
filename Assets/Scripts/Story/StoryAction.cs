using System;
using System.Collections.Generic;
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
        ChooseCardToRemove,
        RemoveRandomElement
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
                SaveSystem.SaveUnlockData();
                break;

            case ActionType.AddCardToDeck:
                InGameCard newCard;
                if (Name != "")
                {
                    newCard = new InGameCard(CardCollection.Instance.FindCardByName(Name));
                }
                else newCard = new InGameCard(CardCollection.Instance.RandomCard());

                for (int i = 0; i < Amount; i++)
                {
                    PlayerData.Deck.Add(newCard);
                }

                string acquireHeader = "Acquire";
                if (Amount > 1) acquireHeader += " x" + Amount.ToString();
                DialogManager.Instance.ShowPopup(acquireHeader, newCard);
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
                if (CardCollection.UnlockDict[Name]) break;
                CardCollection.UnlockCard(Name);
                InGameCard unlockedCard = new InGameCard(CardCollection.Instance.FindCardByName(Name));
                DialogManager.Instance.ShowPopup("Unlock!", unlockedCard);
                SaveSystem.SaveUnlockData();
                break;

            case ActionType.ChooseCardToRemove:
                if (PlayerData.Deck.Count == 0) break;
                DialogManager.Instance.OpenRemoveCardMenu();
                break;

            case ActionType.RemoveRandomElement:

                List<InGameCard> removableCards = PlayerData.Deck;

                Enum.TryParse(Name, out ElementType element);
                if (element != ElementType.None)
                {
                    removableCards = removableCards.FindAll(card => card.Element == element);
                }

                if (removableCards.Count > 0)
                {
                    InGameCard cardToRemove = removableCards[UnityEngine.Random.Range(0, removableCards.Count)];
                    PlayerData.Deck.Remove(cardToRemove);

                    string removeHeader = "Card Removed";
                    DialogManager.Instance.ShowPopup(removeHeader, cardToRemove);
                }
                else
                {
                    // penalty
                    PlayerData.Health -= Amount;
                    PlayerData.MaxHealth -= Amount;
                    if (PlayerData.Health <= 0) PlayerData.Health = 1;
                    BackgroundShake.Instance.Shake();
                }
                SaveSystem.Save();
                break;
        }
    }
}
