using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCard : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private RectTransform _dragRectTransform;
    [SerializeField]
    private Canvas _canvas;

    private CanvasGroup _canvasGroup;
    private int _siblingIndex;

    private void Awake()
    {
        if (_dragRectTransform == null)
        {
            _dragRectTransform = transform.GetComponent<RectTransform>();
        }

        if (_canvas == null)
        {
            Transform testCanvasTransform = transform.parent;
            while (testCanvasTransform != null)
            {
                _canvas = testCanvasTransform.GetComponent<Canvas>();
                if (_canvas != null)
                {
                    break;
                }
                testCanvasTransform = testCanvasTransform.parent;
            }
        }
        _canvasGroup = GetComponent<CanvasGroup>();
        _siblingIndex = transform.GetSiblingIndex();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _dragRectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _dragRectTransform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _dragRectTransform.SetAsLastSibling();
        _dragRectTransform.localScale = new Vector3(1.05f, 1.05f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _dragRectTransform.SetSiblingIndex(_siblingIndex);
        _dragRectTransform.localScale = new Vector3(1f, 1f, 1f);
    }
}
