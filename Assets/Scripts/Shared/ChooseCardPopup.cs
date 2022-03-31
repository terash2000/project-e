using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChooseCardPopup : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup cardContainer;
    private int amount = 3;

    public void Init()
    {
        for (int i = 0; i < cardContainer.transform.childCount; i++)
        {
            Destroy(cardContainer.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < amount; i++)
        {
            Card card = CardCollection.Instance.RandomCard();

            GameObject cardObj = Instantiate(CardCollection.Instance.CardPrefab, cardContainer.transform);
            cardObj.AddComponent(typeof(ChooseCardHandle));
            cardObj.GetComponent<ChooseCardHandle>().card = card;
            cardObj.GetComponent<CardDisplay>().card = card;
        }
    }

    public void NextScene()
    {
        SceneChanger.NextScene();
    }
}
