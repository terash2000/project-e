using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragCard : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Transform _handPanel;
    private RectTransform _dragRectTransform;
    private Canvas _canvas;
    private GameObject _placeholder;

    // A card placeholder that take a space of hand panel when the actual card is being dragged aroud
    public GameObject placeholder { get { return _placeholder; } }
    public Transform handPanel { get { return _handPanel; } }

    private void Start()
    {
        _dragRectTransform = transform.GetComponent<RectTransform>();
        _canvas = transform.root.GetComponent<Canvas>();
        _handPanel = transform.parent;
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
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Put the card back to the hand panel
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.SetParent(_handPanel);
        transform.SetSiblingIndex(_placeholder.transform.GetSiblingIndex());

        Destroy(_placeholder);
    }
}
