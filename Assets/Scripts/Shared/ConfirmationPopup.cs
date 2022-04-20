using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _confirmButton;

    public void Init(string message, UnityAction Listener)
    {
        _text.text = message;
        _confirmButton.onClick.AddListener(Listener);
        _confirmButton.onClick.AddListener(() => HidePopup());
    }

    public void HidePopup()
    {
        Destroy(gameObject);
    }
}
