using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour
{
    private const int CONTAINER_SIZE = 10;

    [SerializeField]
    private GridLayoutGroup _cardContainer;
    [SerializeField]
    private Button _previousButton;
    [SerializeField]
    private Button _nextButton;

    private int _currentPage = 0;
    private int _maxPage;

    void Start()
    {
        _maxPage = (CardCollection.Instance.allCards.Count - 1) / CONTAINER_SIZE;
        RenderCard();
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
            if (cardIndex >= CardCollection.Instance.allCards.Count) break;
            Card card = CardCollection.Instance.allCards[cardIndex];

            if (CardCollection.UnlockDict[card.CardName])
            {
                GameObject cardObj = Instantiate(CardCollection.Instance.CardPrefab, _cardContainer.transform);
                cardObj.GetComponent<CardDisplay>().Card = card;
            }
            else
            {
                GameObject cardObj = Instantiate(CardCollection.Instance.LockedCardPrefab, _cardContainer.transform);
            }
        }
    }
}
