using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckList : MonoBehaviour
{
    private const int CONTAINER_SIZE = 10;

    [SerializeField] private GridLayoutGroup _cardContainer;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _nextButton;
    private int _currentPage;
    private int _maxPage = 1;

    private List<Card> deck; // temp

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
        GameManager.gameState = GameState.Pause;

        _currentPage = 0;
        _maxPage = (deck.Count - 1) / CONTAINER_SIZE;
        RenderCard();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameManager.gameState = GameState.Running;
    }

    public void NextPage()
    {
        _currentPage += 1;
        RenderCard();
    }

    public void PreviousPage()
    {
        _currentPage -= 1;
        RenderCard();
    }

    private void RenderCard()
    {
        _previousButton.gameObject.SetActive(_currentPage > 0);
        _nextButton.gameObject.SetActive(_currentPage < _maxPage);

        // clear container
        for (int i = 0; i < _cardContainer.transform.childCount; i++)
        {
            Destroy(_cardContainer.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < CONTAINER_SIZE; i++)
        {
            int cardIndex = i + _currentPage * CONTAINER_SIZE;
            if (cardIndex >= deck.Count) break;
            GameObject cardObj = Instantiate(CardCollection.Instance.CardPrefab, _cardContainer.transform);
            cardObj.GetComponent<CardDisplay>().Card = deck[cardIndex];
        }
    }
}
