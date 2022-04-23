using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class CardManager : MonoBehaviourSingleton<CardManager>, ITurnHandler
{
    private const int START_HAND_AMOUNT = 5;
    private const int MAX_HAND_SIZE = 9;
    private const int CARD_PER_TURN = 2;

    public const float ZOOM_CARD_SCALE = 1.1f;
    private const float PREVIEW_DELAY = 0.8f;
    private const float FADING_SPEED = 10f;
    private const float DRAW_COOLDOWN = 0.2f;
    private const float DRAW_ANIMATION_SCALE = 1.4f;
    private const float SINKING_SPEED = 3f;

    [SerializeField] private HorizontalLayoutGroup _handPanel;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Button _deckButton;
    [SerializeField] private Button _gravyardButton;
    [SerializeField] private TextMeshProUGUI _deckText;
    [SerializeField] private TextMeshProUGUI _gravyardText;
    [SerializeField] private CardPage _cardPage;
    [SerializeField] private CardDisplay _previewCard;

    private GameObject _previewContainer;
    private List<InGameCard> _deck = new List<InGameCard>();
    private List<InGameCard> _hand = new List<InGameCard>();
    private List<InGameCard> _graveyard = new List<InGameCard>();
    private bool _isDraggingCard = false;
    private DragCard _selectingCard;
    private DragCard _hoveringCard;
    private Color _black = new Color(0f, 0f, 0f);
    private Color _red = new Color(1f, 0f, 0f);
    private float _previewAlpha = 1f;
    private Queue<GameObject> _drawQueue = new Queue<GameObject>();
    private float _drawCooldown = 0f;

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
        _previewContainer = _previewCard.transform.parent.gameObject;

        _deck = new List<InGameCard>();
        foreach (InGameCard card in PlayerData.Deck)
        {
            _deck.Add(new InGameCard(card));
        }

        ShuffleDeck();

        for (int i = 0; i < START_HAND_AMOUNT - CARD_PER_TURN; i++)
        {
            DrawCard();
        }
    }
    void Update()
    {
        if (GameManager.GameState != GameState.Running) return;

        if (Input.GetMouseButtonUp(0) && IsSelectingCard() && _hoveringCard != _selectingCard)
        {
            Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int mousePos = Arena.Instance.Grid.WorldToCell(new Vector3(oriPos.x, oriPos.y, 0));
            CastCard(_selectingCard.Card, mousePos);

            _selectingCard.HidePreviewCardEffect();
            _selectingCard.IsSelected = false;
            _selectingCard = null;
        }

        // Draw animation
        if (_drawCooldown > 0)
        {
            _drawCooldown -= Time.deltaTime;
        }
        else if (_drawQueue.Count != 0)
        {
            StartCoroutine(DrawAnimation(_drawQueue.Dequeue()));
            _drawCooldown = DRAW_COOLDOWN;
        }

        // Mana color
        for (int i = 0; i < _handPanel.transform.childCount; i++)
        {
            CardDisplay cardDisplay = _handPanel.transform.GetChild(i).GetComponent<CardDisplay>();
            if (cardDisplay == null) continue;

            if (cardDisplay.Card.IsCastable())
            {
                cardDisplay.ManaText.color = _black;
            }
            else
            {
                cardDisplay.ManaText.color = _red;
            }
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

        _graveyard.Add(card);
        _gravyardButton.interactable = true;
        _gravyardText.text = _graveyard.Count.ToString();
    }

    public void DrawCard()
    {
        if (_hand.Count >= MAX_HAND_SIZE) return;

        if (_deck.Count == 0)
        {
            RefillDeck();
            _gravyardButton.interactable = false;
            _gravyardText.text = "0";
        }
        if (_deck.Count > 0)
        {
            AddCardToHand(_deck[0]);
            _deck.RemoveAt(0);
        }
        _deckButton.interactable = _deck.Count > 0;
        _deckText.text = _deck.Count.ToString();
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
        string header = "Draw Pile";
        _cardPage.Open(_deck, header);
    }

    public void ShowGravyard()
    {
        string header = "Discard Pile";
        _cardPage.Open(_graveyard, header);
    }

    public void Preview(InGameCard card, List<InGameCard> otherCards = null)
    {
        _previewCard.Card = card;
        _previewCard.Render();

        _previewContainer.SetActive(true);

        // clear container
        foreach (CardDisplay cards in GetExtraPreviewCards())
        {
            Destroy(cards.gameObject);
        }

        foreach (InGameCard otherCard in otherCards)
        {
            GameObject cardObj = Instantiate(CardCollection.Instance.CardPrefab, _previewContainer.transform);
            cardObj.GetComponent<CardDisplay>().Card = otherCard;
        }

        // fading in
        if (_previewAlpha >= 1f)
        {
            StartCoroutine(FadingIn(_previewCard.transform.parent.GetComponent<CanvasGroup>()));
        }
        else
        {
            _previewAlpha = -PREVIEW_DELAY;
        }
    }

    public void MovePreviewToHand(int index)
    {
        int i = 0;
        foreach (CardDisplay card in CardManager.Instance.GetExtraPreviewCards())
        {

            if (!AddCardToHand(card.Card, index + i)) return;
            i++;
        }
    }

    private IEnumerator FadingIn(CanvasGroup canvasGroup)
    {
        _previewAlpha = -PREVIEW_DELAY;
        while (_previewAlpha < 1f)
        {
            _previewAlpha += FADING_SPEED * Time.deltaTime;
            canvasGroup.alpha = _previewAlpha;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        canvasGroup.alpha = 1f;
    }

    public void HidePreview()
    {
        _previewCard.transform.parent.gameObject.SetActive(false);
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
                    foreach (Status effect in card.Effects)
                    {
                        monster.GainStatus(effect.type, effect.value);
                    }
                    success = true;
                }
            }
        }
        else if (card.BaseCard.Type == CardType.Skill)
        {
            if (tile != null && Arena.Instance.AreaPosList.Contains(mousePos) && MonsterManager.Instance.FindMonsterByTile(mousePos) == null)
            {
                PlayerManager.Instance.Player.SetMovement(mousePos);
                success = true;
            }
        }

        if (success)
        {
            PlayerData.Mana -= card.ManaCost;
            MoveFromHandToGraveyard(_selectingCard.Card);
            Destroy(_selectingCard.gameObject);
        }

        // auto end turn
        if (OptionMenu.AutoEndTurn && _hand.FindAll(card => card.IsCastable()).Count == 0)
        {
            GameManager.Instance.EndTurn();
        }
    }

    private bool AddCardToHand(InGameCard card, int index = -1)
    {
        if (_hand.Count >= MAX_HAND_SIZE) return false;

        _hand.Add(card);

        GameObject cardObj = Instantiate(_cardPrefab, _handPanel.transform);
        cardObj.GetComponent<CardDisplay>().Card = card;
        if (index >= 0) cardObj.transform.SetSiblingIndex(index);
        cardObj.SetActive(false);

        _drawQueue.Enqueue(cardObj);
        return true;
    }

    private IEnumerator DrawAnimation(GameObject cardObj)
    {
        cardObj.SetActive(true);

        Transform cardTransform = cardObj.transform;
        DragCard dragCard = cardObj.GetComponent<DragCard>();

        float scale = DRAW_ANIMATION_SCALE;
        cardTransform.localScale = new Vector3(scale, scale, 1f);

        while (scale > 1f)
        {
            if (cardTransform == null) yield break;
            scale -= SINKING_SPEED * Time.deltaTime;
            cardTransform.localScale = new Vector3(scale, scale, 1f);
            if (dragCard.IsZoom && scale < ZOOM_CARD_SCALE)
            {
                cardTransform.localScale = new Vector3(ZOOM_CARD_SCALE, ZOOM_CARD_SCALE, 1f);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (dragCard.IsZoom)
        {
            cardTransform.localScale = new Vector3(ZOOM_CARD_SCALE, ZOOM_CARD_SCALE, 1f);
        }
        else
        {
            cardTransform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private List<CardDisplay> GetExtraPreviewCards()
    {
        List<CardDisplay> cards = new List<CardDisplay>();

        for (int i = 0; i < _previewContainer.transform.childCount; i++)
        {
            CardDisplay child = _previewContainer.transform.GetChild(i).GetComponent<CardDisplay>();
            if (child == _previewCard) continue;
            cards.Add(child);
        }

        return cards;
    }
}
