using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public static bool AutoEndTurn;
    public static bool ShowMonstersAttackArea;

    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _seVolumeSlider;
    [SerializeField] private Slider _voiceVolumeSlider;
    [SerializeField] private Toggle _autoEndTurnTog;
    [SerializeField] private Toggle _showAttackAreaTog;
    [SerializeField] private GameObject _confirmationPopup;

    public void Start()
    {
        _masterVolumeSlider.value = SoundController.MasterVolume;
        _bgmVolumeSlider.value = SoundController.BGMVolume;
        _seVolumeSlider.value = SoundController.SEVolume;
        _voiceVolumeSlider.value = SoundController.VoiceVolume;

        _autoEndTurnTog.isOn = AutoEndTurn;
        _showAttackAreaTog.isOn = ShowMonstersAttackArea;
    }

    public void Pause()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        GameManager.GameState = GameState.Pause;
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameManager.GameState = GameState.Running;
    }

    public void TogAutoEndTurn(bool tog)
    {
        AutoEndTurn = tog;
        SaveSystem.SaveOptionMenu();
    }

    public void TogMonstersAttackArea(bool tog)
    {
        ShowMonstersAttackArea = tog;
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
            GameObject newPopup = Instantiate(_confirmationPopup, transform.parent);
            string message = "Are you sure? Abandoning the run will result in a loss";
            UnityAction action = () => Abandon(true);
            newPopup.GetComponent<ConfirmationPopup>().Init(message, action);
        }
    }

    public void ReturnMainMenu()
    {
        Time.timeScale = 1f;
        SceneChanger.Instance.LoadScene("MainMenuScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
