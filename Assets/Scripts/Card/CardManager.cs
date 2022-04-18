using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CardManager : MonoBehaviourSingleton<CardManager>, ITurnHandler
{
    private const int START_HAND_AMOUNT = 5;
    private const int MAX_HAND_SIZE = 9;
    private const int CARD_PER_TURN = 2;

    [SerializeField] private HorizontalLayoutGroup _handPanel;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private CardList _starterDeck;
    [SerializeField] private CanvasGroup _deckUI;
    [SerializeField] private CanvasGroup _gravyardUI;
    [SerializeField] private CardPage _cardPage;
    [SerializeField] private GridLayoutGroup _previewCardContainer;

    private List<InGameCard> _deck = new List<InGameCard>();
    private List<InGameCard> _hand = new List<InGameCard>();
    private List<InGameCard> _graveyard = new List<InGameCard>();

    private bool _isDraggingCard = false;
    private DragCard _selectingCard;
    private DragCard _hoveringCard;
    public bool IsDraggingCard
    {
        get { return _isDraggingCard; }
        set { _isDraggingCard = value; }
    }
    public DragCard SelectingCard
    {
        get { return _selectingCard; }
        set { _selectingCard = value; }
    }
    public DragCard HoveringCard
    {
        get { return _hoveringCard; }
        set { _hoveringCard = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerData.Deck == null)
        {
            PlayerData.Deck = new List<InGameCard>();
            foreach (Card card in _starterDeck.cards)
            {
                PlayerData.Deck.Add(new InGameCard(card));
            }
        }

        _deck = new List<InGameCard>(PlayerData.Deck);
        ShuffleDeck();

        for (int i = 0; i < START_HAND_AMOUNT; i++)
        {
            DrawCard();
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && IsSelectingCard() && _hoveringCard != _selectingCard)
        {
            Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int mousePos = Arena.Instance.Grid.WorldToCell(new Vector3(oriPos.x, oriPos.y, 0));
            CastCard(_selectingCard.Card, mousePos);

            _selectingCard.HidePreviewCardEffect();
            _selectingCard.IsSelected = false;
            _selectingCard = null;
        }
    }

    public void OnStartTurn()
    {
        for (int i = 0; i < CARD_PER_TURN; i++)
        {
            DrawCard();
        }
    }

    public void OnEndTurn()
    {

    }

    public bool IsSelectingCard()
    {
        return _selectingCard != null;
    }

    public bool IsHoveringCard()
    {
        return _hoveringCard != null;
    }

    public void MoveFromHandToGraveyard(InGameCard card)
    {
        _hand.Remove(card);
        AddCardToGraveyard(card);
    }

    public void AddCardToGraveyard(InGameCard card)
    {
        if (card.IsToken) return;

        // clone card
        InGameCard clone = new InGameCard(card);

        _graveyard.Add(clone);
        _gravyardUI.alpha = 1;
        _gravyardUI.blocksRaycasts = true;
    }

    public void DrawCard()
    {
        if (_hand.Count >= MAX_HAND_SIZE) return;

        if (_deck.Count == 0)
        {
            RefillDeck();
            _gravyardUI.alpha = 0;
            _gravyardUI.blocksRaycasts = false;
            _deckUI.alpha = 1;
            _deckUI.blocksRaycasts = true;
        }
        if (_deck.Count > 0)
        {
            _hand.Add(_deck[0]);
            _deck.RemoveAt(0);
            GameObject cardObj = Instantiate(_cardPrefab, _handPanel.transform);
            cardObj.GetComponent<CardDisplay>().Card = _hand[_hand.Count - 1];
        }
        if (_deck.Count == 0)
        {
            _deckUI.alpha = 0;
            _deckUI.blocksRaycasts = false;
        }
    }

    public void RefillDeck()
    {
        (_deck, _graveyard) = (_graveyard, _deck);
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < _deck.Count; i++)
        {
            int randomIndex = Random.Range(i, _deck.Count);
            (_deck[i], _deck[randomIndex]) = (_deck[randomIndex], _deck[i]);
        }
    }

    public void ShowDeck()
    {
        _cardPage.Cards = _deck;
        _cardPage.Open();
    }

    public void ShowGravyard()
    {
        _cardPage.Cards = _graveyard;
        _cardPage.Open();
    }

    public void PreviewCard(InGameCard card)
    {
        for (int i = 0; i < _previewCardContainer.transform.childCount; i++)
        {
            Destroy(_previewCardContainer.transform.GetChild(i).gameObject);
        }
        GameObject cardObj = Instantiate(CardCollection.Instance.CardPrefab, _previewCardContainer.transform);
        cardObj.GetComponent<CardDisplay>().Card = card;

        _previewCardContainer.gameObject.SetActive(true);
    }

    public void HidePreview()
    {
        _previewCardContainer.gameObject.SetActive(false);
    }

    private void CastCard(InGameCard card, Vector3Int mousePos)
    {
        // TODO: cast the card based on the actual card data
        Tile tile = (Tile)Arena.Instance.Tilemap.GetTile(mousePos);
        if (tile == null) return;

        bool success = false;
        if (card.BaseCard.Type == CardType.Attack)
        {
            foreach (Vector3Int pos in Arena.Instance.TargetPosList)
            {
                Monster monster = MonsterManager.Instance.FindMonsterByTile(pos);
                if (monster != null)
                {
                    monster.TakeDamage(card.Damage);
                    foreach (Status effect in card.BaseCard.Effects)
                    {
                        monster.GainStatus(effect.type, effect.value);

                    }
                    success = true;
                }
            }
        }
        else if (card.BaseCard.Type == CardType.Skill)
        {
            PlayerManager.Instance.Player.SetMovement(mousePos);
            success = true;
        }

        if (success)
        {
            PlayerData.Mana -= card.ManaCost;
            MoveFromHandToGraveyard(_selectingCard.Card);
            Destroy(_selectingCard.gameObject);
        }
    }
}
