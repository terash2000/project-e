using UnityEngine;
using UnityEngine.EventSystems;

public class ChooseCardHandle : MonoBehaviour, IPointerClickHandler
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

        SceneChanger.Instance.NextScene();
    }
}
