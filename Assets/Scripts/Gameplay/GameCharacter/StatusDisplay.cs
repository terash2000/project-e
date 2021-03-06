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
                _icon.sprite = GameManager.Instance.AcidIcon;
                _icon.color = GameManager.Instance.AcidColor;
                _amountText.color = GameManager.Instance.AcidColor;
                break;
            case Status.Type.Burn:
                _icon.sprite = GameManager.Instance.BurnIcon;
                _icon.color = GameManager.Instance.BurnColor;
                _amountText.color = GameManager.Instance.BurnColor;
                break;
            case Status.Type.Strong:
                _icon.sprite = GameManager.Instance.StrongIcon;
                _icon.color = GameManager.Instance.StrongColor;
                _amountText.color = GameManager.Instance.StrongColor;
                break;
            case Status.Type.Weak:
                _icon.sprite = GameManager.Instance.WeakIcon;
                _icon.color = GameManager.Instance.WeakColor;
                _amountText.color = GameManager.Instance.WeakColor;
                break;
        }
        _amountText.text = amount.ToString();
    }
}
