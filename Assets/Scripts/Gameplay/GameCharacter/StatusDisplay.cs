using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image icon;

    public void Init(Status.Type status, int amount)
    {
        switch (status)
        {
            case Status.Type.Acid:
                icon.sprite = GameManager.Instance.acidIcon;
                icon.color = GameManager.Instance.acidColor;
                amountText.color = GameManager.Instance.acidColor;
                break;
            case Status.Type.Burn:
                icon.sprite = GameManager.Instance.burnIcon;
                icon.color = GameManager.Instance.burnColor;
                amountText.color = GameManager.Instance.burnColor;
                break;
        }
        amountText.text = amount.ToString();
    }
}
