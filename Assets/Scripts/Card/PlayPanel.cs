using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayPanel : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        DragCard draggedCard = eventData.pointerDrag.GetComponent<DragCard>();
        if (draggedCard == null)
            return;

        InGameCard card = eventData.pointerDrag.GetComponent<CardDisplay>().Card;
        CardManager.Instance.MoveFromHandToGraveyard(card);
        Destroy(draggedCard.Placeholder);
        Destroy(eventData.pointerDrag);
        // TODO: cast the dropped card
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        DragCard draggedCard = eventData.pointerDrag.GetComponent<DragCard>();
        if (draggedCard == null)
            return;

        // Move the dragged card's placeholder out of the hand panel
        draggedCard.Placeholder.transform.SetParent(transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        DragCard draggedCard = eventData.pointerDrag.GetComponent<DragCard>();
        if (draggedCard == null)
            return;

        draggedCard.Placeholder.transform.SetParent(draggedCard.HandPanel);
    }
}
