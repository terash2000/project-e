using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ResetGame : MonoBehaviour
{
    [SerializeField] private GameObject _confirmationPopup;

    public void ResetData(bool confirm = false)
    {
        if (confirm)
        {
            SaveSystem.DeleteSave();
            SaveSystem.DeleteUnlockData();
            SaveSystem.LoadUnlockData();
        }
        else
        {
            GameObject newPopup = Instantiate(_confirmationPopup, transform.parent);
            string message = "Are you sure?\nResetting the game will clear all saved data.";
            UnityAction action = () => ResetData(true);
            newPopup.GetComponent<ConfirmationPopup>().Init(message, action);
        }
    }
}
