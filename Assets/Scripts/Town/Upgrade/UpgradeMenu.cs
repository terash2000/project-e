using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeMenu : CardPage
{

    protected override void RenderCard()
    {
        for (int i = 0; i < CONTAINER_SIZE; i++)
        {
            int cardIndex = i + _currentPage * CONTAINER_SIZE;
            if (cardIndex >= _cards.Count) break;
            GameObject cardObj = Instantiate(CardCollection.Instance.CardPrefab, _cardContainer.transform);
            cardObj.GetComponent<CardDisplay>().Card = _cards[cardIndex];

            Button cardButton = cardObj.AddComponent(typeof(Button)) as Button;

            UnityAction action = () => Upgrade(cardIndex);
            cardButton.onClick.AddListener(action);
        }
    }

    private void Upgrade(int cardIndex, bool confirm = false)
    {
        Debug.Log("Upgrade card-" + cardIndex.ToString());
        Close();
    }
}
