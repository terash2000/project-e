using UnityEngine;

public class CollectionPage : Pagination
{
    void Start()
    {
        _cards = CardCollection.Instance.AllCards;
        _maxPage = (_cards.Count - 1) / CONTAINER_SIZE;
        Render();
    }

    protected override void RenderCard()
    {
        for (int i = 0; i < CONTAINER_SIZE; i++)
        {
            int cardIndex = i + _currentPage * CONTAINER_SIZE;
            if (cardIndex >= _cards.Count) break;
            Card card = _cards[cardIndex];

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
