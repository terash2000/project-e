using UnityEngine;
using UnityEngine.Events;

public class OptionMenu : MonoBehaviour
{
    [SerializeField] private GameObject confirmationPopup;
    public void Pause()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        GameManager.gameState = GameState.Pause;
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameManager.gameState = GameState.Running;
    }

    public void Abandon(bool confirm = false)
    {
        if (confirm)
        {
            SaveSystem.DeleteSave();
            ReturnMainMenu();
        }
        else
        {
            GameObject newPopup = Instantiate(confirmationPopup, transform.parent);
            string message = "Are you sure? Abandoning the run will result in a loss";
            UnityAction action = () => Abandon(true);
            newPopup.GetComponent<ConfirmationPopup>().Init(message, action);
        }
    }

    public void ReturnMainMenu()
    {
        Time.timeScale = 1f;
        SceneChanger.LoadScene("MainMenuScene");
    }

    public void ExitGame()
    {
        SceneChanger.ExitGame();
    }
}
