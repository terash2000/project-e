using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
    public GameObject confirmationPopup;

    public void Abandon(bool confirm = false)
    {
        if (confirm)
        {
            SaveSystem.DeleteSave();
            PlayerData.previousScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("MainMenuScene");
        }
        else
        {
            GameObject newPopup = Instantiate(confirmationPopup, transform.parent);
            string message = "Are you sure? Abandoning the run will result in a loss";
            UnityAction action = () => Abandon(true);
            newPopup.GetComponent<ConfirmationPopup>().Init(message, action);
        }
    }
}
