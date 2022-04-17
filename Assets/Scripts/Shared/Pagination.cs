using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pagination : MonoBehaviour
{
    protected const int CONTAINER_SIZE = 10;

    [SerializeField] protected GridLayoutGroup _cardContainer;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _nextButton;

    protected int _currentPage = 0;
    protected int _maxPage = 1;
    protected List<InGameCard> _cards;

    public List<InGameCard> Cards
    {
        get { return _cards; }
        set { _cards = value; }
    }

    public void NextPage()
    {
        _currentPage += 1;
        Render();
    }

    public void PreviousPage()
    {
        _currentPage -= 1;
        Render();
    }

    protected void Render()
    {
        _previousButton.gameObject.SetActive(_currentPage > 0);
        _nextButton.gameObject.SetActive(_currentPage < _maxPage);

        // clear container
        for (int i = 0; i < _cardContainer.transform.childCount; i++)
        {
            Destroy(_cardContainer.transform.GetChild(i).gameObject);
        }

        RenderCard();
    }

    protected virtual void RenderCard()
    {
        for (int i = 0; i < CONTAINER_SIZE; i++)
        {
            int cardIndex = i + _currentPage * CONTAINER_SIZE;
            if (cardIndex >= _cards.Count) break;
            GameObject cardObj = Instantiate(CardCollection.Instance.CardPrefab, _cardContainer.transform);
            cardObj.GetComponent<CardDisplay>().Card = _cards[cardIndex];
        }
    }
}