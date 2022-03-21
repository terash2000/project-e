using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationPopup : MonoBehaviour
{
    public void Init(string message, UnityAction Listener)
    {
        Text text = gameObject.transform.Find("Text").gameObject.GetComponent<Text>();
        text.text = message;

        Button confirmButton = gameObject.transform.Find("Confirm").gameObject.GetComponent<Button>();
        confirmButton.onClick.AddListener(Listener);
    }
    public void HidePopup()
    {
        Destroy(gameObject);
    }
}
