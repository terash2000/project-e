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
                icon.sprite = GameManager.singleton.AcidIcon;
                icon.color = GameManager.singleton.AcidColor;
                icon.color = GameManager.singleton.AcidColor;
                amountText.color = GameManager.singleton.AcidColor;
                break;
        }
        amountText.text = amount.ToString();
    }
}
