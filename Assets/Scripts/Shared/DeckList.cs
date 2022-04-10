using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckList : Pagination
{
    protected List<Card> deck; // temp

    public void Open()
    {
        // temp deck
        deck = new List<Card>();
        // add 16 cards to test deck
        for (int i = 0; i < 16; i++)
        {
            deck.Add(CardCollection.Instance.AllCards[0]);
        }

        gameObject.SetActive(true);
        Time.timeScale = 0f;
        GameManager.GameState = GameState.Pause;

        _currentPage = 0;
        _maxPage = (deck.Count - 1) / CONTAINER_SIZE;
        Render();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameManager.GameState = GameState.Running;
    }

    protected override void RenderCard()
    {
        for (int i = 0; i < CONTAINER_SIZE; i++)
        {
            int cardIndex = i + _currentPage * CONTAINER_SIZE;
            if (cardIndex >= deck.Count) break;
            GameObject cardObj = Instantiate(CardCollection.Instance.CardPrefab, _cardContainer.transform);
            cardObj.GetComponent<CardDisplay>().Card = deck[cardIndex];
        }
    }
}
