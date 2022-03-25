using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup cardContainer;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject collectionCardPrefab;
    private int currentPage = 0;
    private int maxPage = 1;
    private const int ContainerSize = 10;

    void Start()
    {
        maxPage = (CardCollection.singleton.allCards.Count - 1) / ContainerSize;
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

        for (int i = 0; i < ContainerSize; i++)
        {
            int cardIndex = i + currentPage * ContainerSize;
            if (cardIndex >= CardCollection.singleton.allCards.Count) break;

            GameObject card = Instantiate(collectionCardPrefab, cardContainer.transform);
        }
    }
}
