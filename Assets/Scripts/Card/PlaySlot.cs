using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        DragCard d = eventData.pointerDrag.GetComponent<DragCard>();
        if (d == null)
            return;

        Destroy(d.placeholder);
        Destroy(eventData.pointerDrag);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        DragCard d = eventData.pointerDrag.GetComponent<DragCard>();
        if (d == null)
            return;

        d.placeholder.transform.SetParent(transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        DragCard d = eventData.pointerDrag.GetComponent<DragCard>();
        if (d == null)
            return;

        d.placeholder.transform.SetParent(d.handPanel.transform);
    }
}
