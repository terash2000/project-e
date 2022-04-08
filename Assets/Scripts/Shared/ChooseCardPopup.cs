using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCardPopup : MonoBehaviour
{
    private const int AMOUNT = 3;

    [SerializeField] private GridLayoutGroup _cardContainer;

    public void Init()
    {
        for (int i = 0; i < _cardContainer.transform.childCount; i++)
        {
            Destroy(_cardContainer.transform.GetChild(i).gameObject);
        }
        List<string> cards = new List<string>();
        for (int i = 0; i < AMOUNT; i++)
        {
            Card card = CardCollection.Instance.RandomCard(cards);
            cards.Add(card.CardName);

            GameObject cardObj = Instantiate(CardCollection.Instance.CardPrefab, _cardContainer.transform);
            cardObj.AddComponent(typeof(ChooseCardHandle));
            cardObj.GetComponent<ChooseCardHandle>().Card = card;
            cardObj.GetComponent<CardDisplay>().Card = card;
        }
    }

    public void NextScene()
    {
        SceneChanger.NextScene();
    }
}
