using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CardManager : MonoBehaviourSingleton<CardManager>
{
    [SerializeField] private HorizontalLayoutGroup _handPanel;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private CardList _starterDeck;
    [SerializeField] private CanvasGroup _deckUI;
    [SerializeField] private CanvasGroup _gravyardUI;
    [SerializeField] private CardPage _cardPage;
    private List<InGameCard> _deck = new List<InGameCard>();
    private List<InGameCard> _hand = new List<InGameCard>();
    private List<InGameCard> _graveyard = new List<InGameCard>();

    public bool isSelectingCard = false;
    public bool isDraggingCard = false;
    public bool isMouseHovering = false;
    public DragCard selectingCard;

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

        for (int i = 0; i < 7; i++)
        {
            DrawCard();
        }
    }
    void Update()
    {
        InGameCard card = Arena.Instance.SelectedCard;
        if (Input.GetMouseButtonUp(0) && isSelectingCard && card.isCastable() && !isMouseHovering)
        {
            Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int mousePos = Arena.Instance.Grid.WorldToCell(new Vector3(oriPos.x, oriPos.y, 0));
            castCard(card, mousePos);
            isSelectingCard = false;
            Arena.Instance.SelectedCard = null;
        }
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

    public static void HandleCardHover(InGameCard card)
    {
        Arena.Instance.SelectedCard = card;
        Arena.Instance.ShowRadius(card.BaseCard.AreaShape, card.BaseCard.TargetShape, card.BaseCard.Range);
    }
    public static void HandleCardHoverExit(InGameCard card)
    {
        Arena.Instance.SelectedCard = null;
        Arena.Instance.HideRadius(card.BaseCard.AreaShape, card.BaseCard.Range);
    }

    private void castCard(InGameCard card, Vector3Int mousePos)
    {
        Arena.Instance.HideRadius(card.BaseCard.AreaShape, card.BaseCard.Range);

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
                    monster.TakeDamage(card.BaseCard.Damage);
                    monster.GainStatus(Status.Type.Stun);
                    monster.GainStatus(Status.Type.Acid, 2);
                    monster.GainStatus(Status.Type.Burn, 3);
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
            MoveFromHandToGraveyard(selectingCard.GetComponent<CardDisplay>().Card);
            Destroy(selectingCard.gameObject);
        }
    }
}
