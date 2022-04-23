using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CardCollection : MonoBehaviourSingletonPersistent<CardCollection>
{
    [SerializeField] private List<Card> _allCards;
    [SerializeField] private List<Combo> _allCombo;
    [SerializeField] private GameObject _newCardPopup;
    [SerializeField] private GameObject _chooseCardPopup;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _lockedCardPrefab;

    private static Dictionary<string, bool> _unlockDict;

    public static Dictionary<string, bool> UnlockDict
    {
        get { return _unlockDict; }
        set { _unlockDict = value; }
    }

    public List<Combo> AllCombo { get { return _allCombo; } }
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
        foreach (Card card in _allCards)
        {
            if (card.CardName == cardName) return card;
        }
        return null;
    }

    public Combo FindCombo(ElementType base1, ElementType base2)
    {
        foreach (Combo combo in _allCombo)
        {
            if (combo.Base1 == base1 && combo.Base2 == base2) return combo;
            if (combo.Base1 == base2 && combo.Base2 == base1) return combo;
        }
        return null;
    }

    public Card RandomCard(List<string> exclude = null)
    {
        List<Card> unlockedCards = GetNonSpecialCards().FindAll(card => UnlockDict[card.CardName]);

        if (exclude != null)
        {
            unlockedCards = unlockedCards.FindAll(card => !exclude.Contains(card.CardName));
        }

        return unlockedCards[Random.Range(0, unlockedCards.Count)];
    }

    public Dictionary<string, bool> StarterCards()
    {
        Dictionary<string, bool> dict = new Dictionary<string, bool>();
        foreach (Card card in GetNonSpecialCards())
        {
            dict.Add(card.CardName, card.IsUnlocked);
        }
        return dict;
    }

    public static void UnlockCard(string key)
    {
        UnlockDict[key] = true;
    }

    public List<Card> GetNonSpecialCards()
    {
        return _allCards.FindAll(card => card.Rarity != CardRarity.Special);
    }
}
