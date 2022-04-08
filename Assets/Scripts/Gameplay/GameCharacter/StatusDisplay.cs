using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private Image _icon;

    public void Init(Status.Type status, int amount)
    {
        switch (status)
        {
            case Status.Type.Acid:
                _icon.sprite = GameManager.Instance.acidIcon;
                _icon.color = GameManager.Instance.acidColor;
                _amountText.color = GameManager.Instance.acidColor;
                break;
            case Status.Type.Burn:
                _icon.sprite = GameManager.Instance.burnIcon;
                _icon.color = GameManager.Instance.burnColor;
                _amountText.color = GameManager.Instance.burnColor;
                break;
        }
        _amountText.text = amount.ToString();
    }
}
