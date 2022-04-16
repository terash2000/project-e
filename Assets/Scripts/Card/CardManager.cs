using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup _handPanel;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private CardList _starterDeck;
    [SerializeField] private CanvasGroup _deckUI;
    [SerializeField] private CanvasGroup _gravyardUI;
    private List<Card> _deck = new List<Card>();
    private List<Card> _hand = new List<Card>();
    private List<Card> _graveyard = new List<Card>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Card card in _starterDeck.cards)
        {
            _deck.Add(card);

        }
        ShuffleDeck();

        for (int i = 0; i < 7; i++)
        {
            DrawCard();
        }
    }

    public void MoveFromHandToGraveyard(Card card)
    {
        _hand.Remove(card);
        _graveyard.Add(card);
        _gravyardUI.alpha = 1;
        _gravyardUI.blocksRaycasts = true;
    }

    public void DrawCard()
    {
        if (_deck.Count == 0)
        {
            RefillDeck();
            _gravyardUI.alpha = 0;
            _gravyardUI.blocksRaycasts = false;
            _deckUI.alpha = 1;
            _deckUI.blocksRaycasts = true;
        }
        if (_deck.Count == 0)
        {
            _deckUI.alpha = 0;
            _deckUI.blocksRaycasts = false;
            return;

        }
        _hand.Add(_deck[0]);
        _deck.RemoveAt(0);
        GameObject cardObj = Instantiate(_cardPrefab, _handPanel.transform);
        cardObj.GetComponent<CardDisplay>().Card = _hand[_hand.Count - 1];
    }


    public void RefillDeck()
    {
        (_deck, _graveyard) = (_graveyard, _deck);
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < _deck.Count; i++)
        {
            int randomIndex = Random.Range(i, _deck.Count);
            (_deck[i], _deck[randomIndex]) = (_deck[randomIndex], _deck[i]);
        }
    }

}
