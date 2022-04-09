using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup _handPanel;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Card _cardData;
    private List<Card> _deck = new List<Card>();
    private List<Card> _hand = new List<Card>();
    private List<Card> _graveyard = new List<Card>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 7; i++)
        {
            _deck.Add(_cardData);
        }
        RenderCard();
    }

    public void RenderCard()
    {
        foreach (Transform child in _handPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Card card in _hand)
        {
            GameObject cardObj = Instantiate(_cardPrefab, _handPanel.transform);
            cardObj.GetComponent<CardDisplay>().Card = card;
        }
    }

    public void RemoveCardFromHand(Card card)
    {
        int targetIndex = _hand.FindIndex(delegate (Card c) { return c.name == card.name; });
        _hand.Remove(card);
        Destroy(card);
    }

    public void DrawCard()
    {
        if (_deck.Count == 0)
        {
            ResetDeck();
        }
        if (_deck.Count == 0)
        {
            return;
        }
        _hand.Add(_deck[0]);
        _deck.RemoveAt(0);
        GameObject cardObj = Instantiate(_cardPrefab, _handPanel.transform);
        cardObj.GetComponent<CardDisplay>().Card = _hand[_hand.Count - 1];
        // AdjustCardPosition();
    }


    public void ResetDeck()
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

    private void AdjustCardPosition()
    {
        _handPanel.spacing = Mathf.Min(10f, (4 - _hand.Count) * 40f);
    }
}
