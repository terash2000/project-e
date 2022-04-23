using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ComboMenu : MonoBehaviour
{
    [SerializeField] private GameObject _comboPrefab;
    [SerializeField] private VerticalLayoutGroup _comboContainer;
    [SerializeField] private GameObject _detail;
    [SerializeField] private TextMeshProUGUI _equationText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private CardDisplay _card;

    public void Start()
    {
        // clear container
        for (int i = 0; i < _comboContainer.transform.childCount; i++)
        {
            Destroy(_comboContainer.transform.GetChild(i).gameObject);
        }

        foreach (Combo combo in CardCollection.Instance.AllCombo)
        {
            GameObject cardObj = Instantiate(_comboPrefab, _comboContainer.transform);
            cardObj.GetComponent<ComboInfo>().Init(combo);

            UnityAction action = () => ShowDetail(combo);
            cardObj.GetComponent<Button>().onClick.AddListener(action); ;
        }
    }

    public void ShowDetail(Combo combo)
    {
        _detail.SetActive(true);
        _equationText.text = combo.Equation;
        _descriptionText.text = combo.Description.Replace("___", "\n");
        _card.gameObject.SetActive(false);
        foreach (Bonus bonus in combo.AttackCardBonuses.Concat(combo.SkillCardBonuses))
        {
            if (bonus.type == Bonus.Type.CreateCard)
            {
                _card.Card = new InGameCard(bonus.card);
                _card.Render();
                _card.gameObject.SetActive(true);
                break;
            }
        }
    }
}
