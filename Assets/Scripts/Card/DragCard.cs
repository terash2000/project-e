using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragCard : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private const float COMBINE_RANGE = 50f;

    private Transform _handPanel;
    private RectTransform _dragRectTransform;
    private Canvas _canvas;
    private GameObject _placeholder;
    private Color _cyan = new Color(0f, 1f, 1f);
    private Color _white = new Color(1f, 1f, 1f);
    private InGameCard _card;
    private bool _isSelected = false;

    // A card placeholder that take a space of hand panel when the actual card is being dragged aroud
    public GameObject Placeholder { get { return _placeholder; } }
    public Transform HandPanel { get { return _handPanel; } }
    public InGameCard Card { get { return _card; } }
    public bool IsSelected
    {
        get { return _isSelected; }
        set { _isSelected = value; }
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
        if (CardManager.Instance.IsSelectingCard()) return;

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

        // showPreviewCardEffect();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CardManager.Instance.IsSelectingCard()) return;

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

        // Preview combo
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
        if (CardManager.Instance.IsSelectingCard()) return;

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
        InGameCard currentCard = GetComponent<CardDisplay>().Card;

        for (int i = 0; i < _handPanel.childCount; i++)
        {
            CardDisplay cardDisplay = _handPanel.GetChild(i).GetComponent<CardDisplay>();
            if (cardDisplay == null) continue;

            if (Mathf.Abs(transform.position.x - _handPanel.GetChild(i).position.x) < COMBINE_RANGE &&
                    Mathf.Abs(transform.position.y - _handPanel.GetChild(i).position.y) < COMBINE_RANGE)
            {
                Combo combo = CardCollection.Instance.FindCombo(currentCard.Element, cardDisplay.Card.Element);
                if (combo != null)
                {
                    // Remove another card
                    CardManager.Instance.MoveFromHandToGraveyard(cardDisplay.Card);
                    Destroy(cardDisplay.gameObject);

                    // Add old card to graveyard
                    CardManager.Instance.AddCardToGraveyard(currentCard);

                    // Upgrade base card
                    currentCard.IsToken = true;
                    currentCard.Element = combo.Result;

                    List<Bonus> bonuses;
                    if (currentCard.BaseCard.Type == CardType.Attack)
                    {
                        bonuses = combo.AttackCardBonuses;
                    }
                    else
                    {
                        bonuses = combo.SkillCardBonuses;
                    }

                    foreach (Bonus bonus in bonuses)
                    {
                        currentCard.GainBonus(bonus);
                    }
                    GetComponent<CardDisplay>().render();
                }
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
            _isSelected = false;
            CardManager.Instance.SelectingCard = null;
        }
        else if (!CardManager.Instance.IsSelectingCard() && _card.IsCastable())
        {
            _isSelected = true;
            CardManager.Instance.SelectingCard = this;

            ShowPreviewCardEffect();
        }
    }

    public void HidePreviewCardEffect()
    {
        Arena.Instance.HideRadius();
        GetComponent<Image>().color = Color.white;
    }

    private void ShowPreviewCardEffect()
    {
        Arena.Instance.ShowRadius(_card);
        GetComponent<Image>().color = Color.yellow;
    }
}
