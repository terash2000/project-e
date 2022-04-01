using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour
{
    private const int CONTAINER_SIZE = 10;

    [SerializeField] private GridLayoutGroup cardContainer;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    private int currentPage = 0;
    private int maxPage;

    void Start()
    {
        maxPage = (CardCollection.Instance.allCards.Count - 1) / CONTAINER_SIZE;
        RenderCard();
    }

    public void NextPage()
    {
        currentPage += 1;
        RenderCard();
    }

    public void PreviousPage()
    {
        currentPage -= 1;
        RenderCard();
    }

    private void RenderCard()
    {
        previousButton.gameObject.SetActive(currentPage > 0);
        nextButton.gameObject.SetActive(currentPage < maxPage);

        // clear container
        for (int i = 0; i < cardContainer.transform.childCount; i++)
        {
            Destroy(cardContainer.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < CONTAINER_SIZE; i++)
        {
            int cardIndex = i + currentPage * CONTAINER_SIZE;
            if (cardIndex >= CardCollection.Instance.allCards.Count) break;
            Card card = CardCollection.Instance.allCards[cardIndex];

            if (CardCollection.unlockDict[card.cardName])
            {
                GameObject cardObj = Instantiate(CardCollection.Instance.CardPrefab, cardContainer.transform);
                cardObj.GetComponent<CardDisplay>().card = card;
            }
            else
            {
                GameObject cardObj = Instantiate(CardCollection.Instance.LockedCardPrefab, cardContainer.transform);
            }
        }
    }
}
