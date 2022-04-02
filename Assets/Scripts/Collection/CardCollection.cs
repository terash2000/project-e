using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCollection : MonoBehaviourSingletonPersistent<CardCollection>
{
    [SerializeField]
    private GameObject _newCardPopup;
    [SerializeField]
    private GameObject _chooseCardPopup;
    [SerializeField]
    private GameObject _cardPrefab;
    [SerializeField]
    private GameObject _lockedCardPrefab;

    public static Dictionary<string, bool> unlockDict;
    public List<Card> allCards;

    public GameObject NewCardPopup { get { return _newCardPopup; } }
    public GameObject ChooseCardPopup { get { return _chooseCardPopup; } }
    public GameObject CardPrefab { get { return _cardPrefab; } }
    public GameObject LockedCardPrefab { get { return _lockedCardPrefab; } }



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
        List<Card> unlockedCards = allCards.FindAll(card => unlockDict[card.name]);
        return unlockedCards[Random.Range(0, unlockedCards.Count)];
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
