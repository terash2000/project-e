using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image icon;

    public void Init(Status status, int amount)
    {
        switch (status)
        {
            case Status.Acid:
                icon.sprite = GameManager.Instance.acidIcon;
                icon.color = GameManager.Instance.acidColor;
                amountText.color = GameManager.Instance.acidColor;
                break;
            case Status.Burn:
                icon.sprite = GameManager.Instance.burnIcon;
                icon.color = GameManager.Instance.burnColor;
                amountText.color = GameManager.Instance.burnColor;
                break;
        }
        amountText.text = amount.ToString();
    }
}
