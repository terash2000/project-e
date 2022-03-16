using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class OptionMenu : MonoBehaviour
{
    public GameObject confirmationPopup;

    public void Pause()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Abandon(bool confirm = false)
    {
        if (confirm)
        {
            SaveSystem.DeleteSave();
            SceneChanger.previousScene = SceneManager.GetActiveScene().name;
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
