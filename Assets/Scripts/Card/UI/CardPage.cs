using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CardPage : Pagination
{
    [SerializeField] private TextMeshProUGUI _header;

    public void OpenPlayerDeck()
    {
        string header = "Deck";
        Open(PlayerData.Deck, header);
    }

    public void Open(List<InGameCard> cards, string header = null)
    {
        _cards = cards;
        if (header != null) _header.text = header;

        _cards = _cards.OrderBy(card => card.BaseCard.ManaCost)
                .ThenBy(card => card.BaseCard.Rarity)
                .ThenBy(card => card.BaseCard.CardName)
                .ThenBy(card => card.IsUpgraded).ToList();

        gameObject.SetActive(true);
        SoundController.Pause();
        Time.timeScale = 0f;
        GameManager.GameState = GameState.Pause;

        _currentPage = 0;
        _maxPage = (_cards.Count - 1) / CONTAINER_SIZE;
        Render();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameManager.GameState = GameState.Running;
    }
}
