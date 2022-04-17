using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradePopup : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _oldCardContainer;
    [SerializeField] private GridLayoutGroup _upgradedContainer;
    [SerializeField] private Button _confirmButton;

    public void Init(InGameCard card, UnityAction Listener)
    {
        for (int i = 0; i < _oldCardContainer.transform.childCount; i++)
        {
            Destroy(_oldCardContainer.transform.GetChild(i).gameObject);
        }

        GameObject oldCardObj = Instantiate(CardCollection.Instance.CardPrefab, _oldCardContainer.transform);
        oldCardObj.GetComponent<CardDisplay>().Card = card;

        for (int i = 0; i < _upgradedContainer.transform.childCount; i++)
        {
            Destroy(_upgradedContainer.transform.GetChild(i).gameObject);
        }

        GameObject upgradedCardObj = Instantiate(CardCollection.Instance.CardPrefab, _upgradedContainer.transform);
        InGameCard upgradedCard = new InGameCard(card);
        upgradedCard.Upgrade();
        upgradedCardObj.GetComponent<CardDisplay>().Card = upgradedCard;

        _confirmButton.onClick.AddListener(Listener);
        _confirmButton.onClick.AddListener(() => HidePopup());
    }

    public void HidePopup()
    {
        Destroy(gameObject);
    }
}
