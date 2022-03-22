using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public static bool autoEndTurn;
    public static bool showMonstersAttackArea;
    [SerializeField] private Toggle autoEndTurnTog;
    [SerializeField] private Toggle showAttackAreaTog;
    [SerializeField] private GameObject confirmationPopup;

    public void Start()
    {
        SaveSystem.LoadOptionMenu();
        autoEndTurnTog.isOn = autoEndTurn;
        showAttackAreaTog.isOn = showMonstersAttackArea;
    }

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

    public void TogAutoEndTurn(bool tog)
    {
        autoEndTurn = tog;
        SaveSystem.SaveOptionMenu();
    }

    public void TogMonstersAttackArea(bool tog)
    {
        showMonstersAttackArea = tog;
        SaveSystem.SaveOptionMenu();
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
