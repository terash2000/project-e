using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckList : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup cardContainer;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    private int currentPage = 0;
    private int maxPage = 1;
    private const int ContainerSize = 10;

    private List<Card> deck = new List<Card>(); // temp

    void Start()
    {
        // add 16 cards to test deck
        for (int i = 0; i < 16; i++)
        {
            deck.Add(CardCollection.Instance.allCards[0]);
        }

        maxPage = (deck.Count - 1) / ContainerSize;
        RenderCard();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        GameManager.gameState = GameState.Pause;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameManager.gameState = GameState.Running;
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
            if (cardIndex >= deck.Count) break;
            Card card = deck[cardIndex];
            GameObject cardObj = Instantiate(CardCollection.Instance.cardPrefab, cardContainer.transform);
        }
    }
}
