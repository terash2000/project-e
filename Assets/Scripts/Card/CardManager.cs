using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviourSingleton<CardManager>
{
    [SerializeField] private HorizontalLayoutGroup _handPanel;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private CardList _starterDeck;
    [SerializeField] private CanvasGroup _deckUI;
    [SerializeField] private CanvasGroup _gravyardUI;
    [SerializeField] private CardPage _cardPage;
    private List<InGameCard> _deck = new List<InGameCard>();
    private List<InGameCard> _hand = new List<InGameCard>();
    private List<InGameCard> _graveyard = new List<InGameCard>();

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerData.Deck == null)
        {
            PlayerData.Deck = new List<InGameCard>();
            foreach (Card card in _starterDeck.cards)
            {
                PlayerData.Deck.Add(new InGameCard(card));
            }
        }

        _deck = new List<InGameCard>(PlayerData.Deck);
        ShuffleDeck();

        for (int i = 0; i < 7; i++)
        {
            DrawCard();
        }
    }

    public void MoveFromHandToGraveyard(InGameCard card)
    {
        _hand.Remove(card);
        AddCardToGraveyard(card);
    }

    public void AddCardToGraveyard(InGameCard card)
    {
        if (card.IsToken) return;

        // clone card
        InGameCard clone = new InGameCard(card);

        _graveyard.Add(clone);
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
        if (_deck.Count > 0)
        {
            _hand.Add(_deck[0]);
            _deck.RemoveAt(0);
            GameObject cardObj = Instantiate(_cardPrefab, _handPanel.transform);
            cardObj.GetComponent<CardDisplay>().Card = _hand[_hand.Count - 1];
        }
        if (_deck.Count == 0)
        {
            _deckUI.alpha = 0;
            _deckUI.blocksRaycasts = false;
        }
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

    public void ShowDeck()
    {
        _cardPage.Cards = _deck;
        _cardPage.Open();
    }

    public void ShowGravyard()
    {
        _cardPage.Cards = _graveyard;
        _cardPage.Open();
    }
}
