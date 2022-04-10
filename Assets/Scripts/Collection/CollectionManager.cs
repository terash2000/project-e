using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : Pagination
{
    void Start()
    {
        _maxPage = (CardCollection.Instance.AllCards.Count - 1) / CONTAINER_SIZE;
        Render();
    }

    protected override void RenderCard()
    {
        for (int i = 0; i < CONTAINER_SIZE; i++)
        {
            int cardIndex = i + _currentPage * CONTAINER_SIZE;
            if (cardIndex >= CardCollection.Instance.AllCards.Count) break;
            Card card = CardCollection.Instance.AllCards[cardIndex];

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
