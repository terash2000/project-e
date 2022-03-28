using System.Collections.Generic;
using UnityEngine;

public class CardCollection : MonoBehaviour
{
    public static CardCollection singleton;
    public static Dictionary<string, bool> unlockDict;
    public List<Card> allCards;
    public GameObject newCardPopup;
    public GameObject cardPrefab;
    public GameObject lockedCard;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }

        SaveSystem.LoadUnlockData();
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
