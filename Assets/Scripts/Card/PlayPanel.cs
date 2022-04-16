using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayPanel : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private CardManager _cardManager;
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        DragCard draggedCard = eventData.pointerDrag.GetComponent<DragCard>();
        if (draggedCard == null)
            return;

        Card card = eventData.pointerDrag.GetComponent<CardDisplay>().Card;
        _cardManager.MoveFromHandToGraveyard(card);
        Destroy(draggedCard.placeholder);
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
        draggedCard.placeholder.transform.SetParent(transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        DragCard draggedCard = eventData.pointerDrag.GetComponent<DragCard>();
        if (draggedCard == null)
            return;

        draggedCard.placeholder.transform.SetParent(draggedCard.handPanel);
    }
}
