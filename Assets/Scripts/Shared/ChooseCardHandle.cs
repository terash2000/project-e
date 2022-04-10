using UnityEngine;
using UnityEngine.EventSystems;

public class ChooseCardHandle : MonoBehaviour, IPointerClickHandler
{
    private Card _card;
    public Card Card
    {
        get { return _card; }
        set { _card = value; }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // TODO add the card to deck
        SceneChanger.Instance.NextScene();
    }
}
