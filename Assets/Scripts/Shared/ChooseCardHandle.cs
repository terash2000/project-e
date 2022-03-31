using UnityEngine;
using UnityEngine.EventSystems;

public class ChooseCardHandle : MonoBehaviour, IPointerClickHandler
{
    public Card card;

    public void OnPointerClick(PointerEventData eventData)
    {
        // TODO add the card to deck
        SceneChanger.NextScene();
    }
}
