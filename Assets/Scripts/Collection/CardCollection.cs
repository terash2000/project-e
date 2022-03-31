using System.Collections.Generic;
using UnityEngine;

public class CardCollection : MonoBehaviourSingletonPersistent<CardCollection>
{
    public static Dictionary<string, bool> unlockDict;
    public List<Card> allCards;
    public GameObject newCardPopup;
    public GameObject chooseCardPopup;
    public GameObject cardPrefab;
    public GameObject lockedCard;

    public override void Awake()
    {
        base.Awake();
        SaveSystem.LoadUnlockData();
    }

    public Card FindCardByName(string cardName)
    {
        foreach (Card card in allCards)
        {
            if (card.cardName == cardName) return card;
        }
        return null;
    }

    public Card RandomCard()
    {
        return allCards[Random.Range(0, allCards.Count)];
    }

    public Dictionary<string, bool> StarterCards()
    {
        Dictionary<string, bool> dict = new Dictionary<string, bool>();
        foreach (Card card in allCards)
        {
            dict.Add(card.cardName, !card.needToUnlock);
        }
        return dict;
    }

    public void UnlockCard(string key)
    {
        unlockDict[key] = true;
        SaveSystem.SaveUnlockData();
    }
}
