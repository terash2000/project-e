using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterStatus : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image icon;

    public void Init(Status status, int amount)
    {
        switch (status)
        {
            case Status.Acid:
                icon.sprite = GameManager.singleton.acidIcon;
                icon.color = GameManager.singleton.acidColor;
                amountText.color = GameManager.singleton.acidColor;
                break;
            case Status.Burn:
                icon.sprite = GameManager.singleton.burnIcon;
                icon.color = GameManager.singleton.burnColor;
                amountText.color = GameManager.singleton.burnColor;
                break;
        }
        amountText.text = amount.ToString();
    }
}
