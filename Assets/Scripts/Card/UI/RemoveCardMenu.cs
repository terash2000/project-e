using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RemoveCardMenu : CardPage
{
    [SerializeField] private GameObject _confirmationPopup;

    public void OpenRemoveCardMenu()
    {
        Open(PlayerData.Deck);
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

            EventTrigger trigger = cardObj.AddComponent(typeof(EventTrigger)) as EventTrigger;
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { cardObj.GetComponent<CardDisplay>().Highlight(); SoundController.Play(SoundCollection.Instance.GetSound("CardHover")); });
            trigger.triggers.Add(entryEnter);

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => { cardObj.GetComponent<CardDisplay>().Unhighlight(); });
            trigger.triggers.Add(entryExit);
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
