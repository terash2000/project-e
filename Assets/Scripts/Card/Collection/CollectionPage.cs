using System.Linq;
using UnityEngine;

public class CollectionPage : Pagination
{
    void Start()
    {
        _cards = CardCollection.Instance.GetNonSpecialCards().Select(baseCard => new InGameCard(baseCard)).ToList();
        _cards = _cards.OrderBy(card => card.BaseCard.ManaCost)
                .ThenBy(card => card.BaseCard.Rarity)
                .ThenBy(card => card.BaseCard.CardName).ToList();

        _maxPage = (_cards.Count - 1) / CONTAINER_SIZE;
        Render();
    }

    protected override void RenderCard()
    {
        for (int i = 0; i < CONTAINER_SIZE; i++)
        {
            int cardIndex = i + _currentPage * CONTAINER_SIZE;
            if (cardIndex >= _cards.Count) break;
            InGameCard card = _cards[cardIndex];

            if (CardCollection.UnlockDict[card.BaseCard.CardName])
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
