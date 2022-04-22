using UnityEngine;
using UnityEngine.EventSystems;

public class ChooseCardHandle : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private InGameCard _card;
    public InGameCard Card
    {
        get { return _card; }
        set { _card = value; }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // add the card to deck
        PlayerData.Deck.Add(_card);
        SoundController.Play(SoundCollection.Instance.GetSound("CardClick"));

        SceneChanger.Instance.NextScene();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundController.Play(SoundCollection.Instance.GetSound("CardHover"));
        GetComponent<CardDisplay>().Highlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<CardDisplay>().Unhighlight();
    }
}
