using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeMenu : CardPage
{
    [SerializeField] private GameObject _upgradePopup;

    public void OpenUpgradeMenu()
    {
        _cards = PlayerData.Deck.FindAll(card => !card.IsUpgraded);
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

            UnityAction action = () => Upgrade(_cards[cardIndex]);
            cardButton.onClick.AddListener(action);
        }
    }

    private void Upgrade(InGameCard card, bool confirm = false)
    {
        if (confirm)
        {
            card.Upgrade();
            SaveSystem.Save();
            Close();
        }
        else
        {
            GameObject newPopup = Instantiate(_upgradePopup, transform.parent);
            UnityAction action = () => Upgrade(card, true);
            newPopup.GetComponent<UpgradePopup>().Init(card, action);
        }
    }
}
