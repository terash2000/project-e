using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RemoveCardMenu : CardPage
{
    [SerializeField] private GameObject _confirmationPopup;

    public void OpenRemoveCardMenu()
    {
        _cards = PlayerData.Deck;
        Open();
    }

    protected override void RenderCard()
    {
        for (int i = 0; i < CONTAINER_SIZE; i++)
        {
            int cardIndex = i + _currentPage * CONTAINER_SIZE;
            if (cardIndex >= _cards.Count) break;
            GameObject cardObj = Instantiate(CardCollection.Instance.CardPrefab, _cardContainer.transform);
            cardObj.GetComponent<CardDisplay>().Card = _cards[cardIndex];

            Button cardButton = cardObj.AddComponent(typeof(Button)) as Button;
            UnityAction action = () =>
            {
                Remove(cardIndex);
                SoundController.Play(SoundCollection.Instance.GetSound("CardClick"));
            };
            cardButton.onClick.AddListener(action);
        }
    }

    private void Remove(int cardIndex, bool confirm = false)
    {
        if (confirm)
        {
            PlayerData.Deck.RemoveAt(cardIndex);
            Close();
        }
        else
        {
            GameObject newPopup = Instantiate(_confirmationPopup, transform.parent);
            UnityAction action = () => Remove(cardIndex, true);
            string message = "Do you want to remove this card?";
            newPopup.GetComponent<ConfirmationPopup>().Init(message, action);
        }
    }
}
