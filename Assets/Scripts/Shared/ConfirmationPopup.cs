using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button confirmButton;

    public void Init(string message, UnityAction Listener)
    {
        text.text = message;
        confirmButton.onClick.AddListener(Listener);
    }
    public void HidePopup()
    {
        Destroy(gameObject);
    }
}
