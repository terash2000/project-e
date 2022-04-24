using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewCardPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private GridLayoutGroup _cardContainer;

    public void Update()
    {
        Time.timeScale = 0f;
    }

    public void Init(string message, InGameCard card)
    {
        _header.text = message;

        for (int i = 0; i < _cardContainer.transform.childCount; i++)
        {
            Destroy(_cardContainer.transform.GetChild(i).gameObject);
        }
        GameObject cardObj = Instantiate(CardCollection.Instance.CardPrefab, _cardContainer.transform);
        cardObj.GetComponent<CardDisplay>().Card = card;
    }

    public void HidePopup()
    {
        Destroy(gameObject);
        Time.timeScale = 1f;
    }
}
