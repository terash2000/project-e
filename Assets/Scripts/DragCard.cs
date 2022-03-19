using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCard : MonoBehaviour,IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform dragRectTransform;
    [SerializeField] private Canvas canvas;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (dragRectTransform == null)
        {
            dragRectTransform = transform.GetComponent<RectTransform>();
        }
        
        if (canvas == null)
        {
            Transform testCanvasTransform = transform.parent;
            while (testCanvasTransform != null)
            {
                canvas = testCanvasTransform.GetComponent<Canvas>();
                if (canvas != null)
                {
                    break;
                }
                testCanvasTransform = testCanvasTransform.parent;
            }
        }
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragRectTransform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
    }
}
