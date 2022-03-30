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
            GameObject cardObj = Instantiate(CardCollection.singleton.cardPrefab, cardContainer.transform);
            cardObj.AddComponent(typeof(ChooseCardHandle));
        }
    }

    public void NextScene()
    {
        SceneChanger.NextScene();
    }
}
