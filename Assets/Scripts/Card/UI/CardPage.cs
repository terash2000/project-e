using System.Collections.Generic;
using UnityEngine;

public class CardPage : Pagination
{
    public void OpenPlayerDeck()
    {
        _cards = PlayerData.Deck;
        Open();
    }

    public void Open()
    {
        gameObject.SetActive(true);
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
