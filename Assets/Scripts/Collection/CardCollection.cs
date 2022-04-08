using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCollection : MonoBehaviourSingletonPersistent<CardCollection>
{
    public List<Card> AllCards;
    [SerializeField]
    private GameObject _newCardPopup;
    [SerializeField]
    private GameObject _chooseCardPopup;
    [SerializeField]
    private GameObject _cardPrefab;
    [SerializeField]
    private GameObject _lockedCardPrefab;

    private static Dictionary<string, bool> _unlockDict;

    public static Dictionary<string, bool> UnlockDict
    {
        get { return _unlockDict; }
        set { _unlockDict = value; }
    }

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
        foreach (Card card in AllCards)
        {
            if (card.CardName == cardName) return card;
        }
        return null;
    }

    public Card RandomCard(List<string> exclude = null)
    {
        List<Card> unlockedCards = AllCards.FindAll(card => UnlockDict[card.CardName]);

        if (exclude != null)
        {
            unlockedCards = unlockedCards.FindAll(card => !exclude.Contains(card.CardName));
        }

        return unlockedCards[Random.Range(0, unlockedCards.Count)];
    }

    public Dictionary<string, bool> StarterCards()
    {
        Dictionary<string, bool> dict = new Dictionary<string, bool>();
        foreach (Card card in AllCards)
        {
            dict.Add(card.CardName, card.IsUnlocked);
        }
        return dict;
    }

    public void UnlockCard(string key)
    {
        UnlockDict[key] = true;
        SaveSystem.SaveUnlockData();
    }
}
