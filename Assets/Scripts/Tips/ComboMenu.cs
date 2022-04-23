using UnityEngine;
using UnityEngine.UI;

public class ComboMenu : MonoBehaviour
{
    [SerializeField] private GameObject _comboPrefab;
    [SerializeField] protected VerticalLayoutGroup _comboContainer;

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
        }
    }
}
