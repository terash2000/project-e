using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewCardPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private GridLayoutGroup cardContainer;

    public void Init(string message, string cardName)
    {
        header.text = message;

        for (int i = 0; i < cardContainer.transform.childCount; i++)
        {
            Destroy(cardContainer.transform.GetChild(i).gameObject);
        }
        GameObject cardObj = Instantiate(CardCollection.Instance.CardPrefab, cardContainer.transform);
        cardObj.GetComponent<CardDisplay>().card = CardCollection.Instance.FindCardByName(cardName);

        Time.timeScale = 0f;
    }
    public void HidePopup()
    {
        Destroy(gameObject);
        Time.timeScale = 1f;
    }
}
