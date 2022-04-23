using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragCard : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private const float COMBINE_RANGE = 80f;

    private Transform _handPanel;
    private RectTransform _dragRectTransform;
    private Canvas _canvas;
    private GameObject _placeholder;
    private Color _cyan = new Color(0f, 1f, 1f);
    private Color _white = new Color(1f, 1f, 1f);
    private InGameCard _card;
    private CardDisplay _cardToCombine;
    private bool _isSelected = false;
    private bool _isZoom;

    // A card placeholder that take a space of hand panel when the actual card is being dragged aroud
    public GameObject Placeholder { get { return _placeholder; } }
    public Transform HandPanel { get { return _handPanel; } }
    public InGameCard Card { get { return _card; } }
    public bool IsSelected
    {
        get { return _isSelected; }
        set { _isSelected = value; }
    }
    public bool IsZoom
    {
        get { return _isZoom; }
    }

    private void Start()
    {
        _dragRectTransform = transform.GetComponent<RectTransform>();
        _canvas = transform.root.GetComponent<Canvas>();
        _handPanel = transform.parent;
        _card = GetComponent<CardDisplay>().Card;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // adjust the position of the card
        _dragRectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;

        // calculate the appropriate sibling index for the placeholder
        int newSiblingIndex = _handPanel.childCount;
        for (int i = 0; i < _handPanel.childCount; i++)
        {
            if (transform.position.x < _handPanel.GetChild(i).position.x)
            {
                newSiblingIndex = i;
                if (_placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex--;
                break;
            }
        }
        _placeholder.transform.SetSiblingIndex(newSiblingIndex);

        // preview combo
        for (int i = 0; i < _handPanel.childCount; i++)
        {
            CardDisplay cardDisplay = _handPanel.GetChild(i).GetComponent<CardDisplay>();
            if (cardDisplay == null) continue;

            if (Mathf.Abs(transform.position.x - _handPanel.GetChild(i).position.x) < COMBINE_RANGE &&
                    Mathf.Abs(transform.position.y - _handPanel.GetChild(i).position.y) < COMBINE_RANGE)
            {
                if (_cardToCombine == cardDisplay)
                {
                    return;
                }
                _cardToCombine = cardDisplay;

                Combo combo = CardCollection.Instance.FindCombo(_card.Element, cardDisplay.Card.Element);
                if (combo != null)
                {
                    InGameCard upgradedCard = new InGameCard(_card);
                    List<InGameCard> createdCards = upgradedCard.GainComboBonus(combo);
                    CardManager.Instance.Preview(upgradedCard, createdCards);
                }
                return;
            }
        }
        _cardToCombine = null;
        CardManager.Instance.HidePreview();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CardManager.Instance.IsSelectingCard())
        {
            _isSelected = false;
            CardManager.Instance.SelectingCard = null;
        }

        SoundController.Play(SoundCollection.Instance.GetSound("CardHover"));
        // Insert a new place holder to the hand panel
        _placeholder = new GameObject();
        _placeholder.transform.SetParent(_handPanel);
        _placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
        LayoutElement new_le = _placeholder.AddComponent<LayoutElement>();
        LayoutElement orignal = GetComponent<LayoutElement>();
        new_le.preferredWidth = orignal.preferredWidth;
        new_le.preferredHeight = orignal.preferredHeight;
        new_le.flexibleWidth = 0;
        new_le.flexibleHeight = 0;

        // Pop the dragged card out of the hand panel
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        transform.SetParent(_handPanel.parent);

        // Highlight combo
        InGameCard currentCard = GetComponent<CardDisplay>().Card;

        for (int i = 0; i < _handPanel.childCount; i++)
        {
            CardDisplay cardDisplay = _handPanel.GetChild(i).GetComponent<CardDisplay>();
            if (cardDisplay != null)
            {
                Combo combo = CardCollection.Instance.FindCombo(currentCard.Element, cardDisplay.Card.Element);
                if (combo != null)
                {
                    cardDisplay.Highlight();
                }
            }
        }
        CardManager.Instance.IsDraggingCard = true;
        ShowPreviewCardEffect();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CardManager.Instance.IsDraggingCard) return;

        // Remove combo highlight
        for (int i = 0; i < _handPanel.childCount; i++)
        {
            CardDisplay cardDisplay = _handPanel.GetChild(i).GetComponent<CardDisplay>();
            if (cardDisplay != null)
            {
                cardDisplay.Unhighlight();
            }
        }

        // Combine card
        if (_cardToCombine != null)
        {
            CardManager.Instance.HidePreview();

            Combo combo = CardCollection.Instance.FindCombo(_card.Element, _cardToCombine.Card.Element);
            if (combo != null)
            {
                // Remove another card
                CardManager.Instance.MoveFromHandToGraveyard(_cardToCombine.Card);
                Destroy(_cardToCombine.gameObject);

                // Add old card to graveyard
                CardManager.Instance.AddCardToGraveyard(new InGameCard(_card));

                // Upgrade base card
                _card.IsToken = true;
                _card.GainComboBonus(combo);
                GetComponent<CardDisplay>().render();

                // Add other created cards to hand
                CardManager.Instance.MovePreviewToHand(_placeholder.transform.GetSiblingIndex() + 1);
            }
        }

        // Put the card back to the hand panel
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.SetParent(_handPanel);
        transform.SetSiblingIndex(_placeholder.transform.GetSiblingIndex());

        Destroy(_placeholder);

        CardManager.Instance.IsDraggingCard = false;
        HidePreviewCardEffect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CardManager.Instance.HoveringCard = this;
        if (CardManager.Instance.IsSelectingCard() || CardManager.Instance.IsDraggingCard) return;
        ShowPreviewCardEffect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CardManager.Instance.HoveringCard = null;
        if (CardManager.Instance.IsSelectingCard() || CardManager.Instance.IsDraggingCard) return;
        HidePreviewCardEffect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isSelected)
        {
            SoundController.Play(SoundCollection.Instance.GetSound("CardHover"));
            _isSelected = false;
            CardManager.Instance.SelectingCard = null;
        }
        else if (_card.IsCastable())
        {
            SoundController.Play(SoundCollection.Instance.GetSound("CardHover"));
            if (CardManager.Instance.IsSelectingCard())
            {
                CardManager.Instance.SelectingCard.HidePreviewCardEffect();
                CardManager.Instance.SelectingCard.IsSelected = false;
            }

            _isSelected = true;
            CardManager.Instance.SelectingCard = this;

            ShowPreviewCardEffect();
        }
    }

    public void HidePreviewCardEffect()
    {
        Arena.Instance.HideRadius();
        GetComponent<Image>().color = Color.white;

        _isZoom = false;
        if (Mathf.Approximately(gameObject.transform.localScale.x, CardManager.ZOOM_CARD_SCALE))
        {
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void ShowPreviewCardEffect()
    {
        Arena.Instance.ShowRadius(_card);
        GetComponent<Image>().color = Color.yellow;

        _isZoom = true;
        if (Mathf.Approximately(gameObject.transform.localScale.x, 1f))
        {
            float scale = CardManager.ZOOM_CARD_SCALE;
            gameObject.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
