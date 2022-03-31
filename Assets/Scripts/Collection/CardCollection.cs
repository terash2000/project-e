using System.Collections.Generic;
using UnityEngine;

public class CardCollection : MonoBehaviourSingleton<CardCollection>
{
    public static Dictionary<string, bool> unlockDict;
    public List<Card> allCards;
    public GameObject newCardPopup;
    public GameObject chooseCardPopup;
    public GameObject cardPrefab;
    public GameObject lockedCard;

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
