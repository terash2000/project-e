using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GameResultPopup : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Image bg;
    private Button confirmButton;

    void Start()
    {
        GameObject panel = transform.Find("Panel").gameObject;
        text = panel.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        bg = panel.GetComponent<Image>();
        confirmButton = panel.transform.Find("Confirm").gameObject.GetComponent<Button>();
        gameObject.SetActive(false);
    }

    public void onLose()
    {
        text.text = "DEFEAT";
        bg.color = Color.red;
        UnityAction action = () => ChangeScene("MainMenuScene");
        confirmButton.onClick.AddListener(action);
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void onWin()
    {
        text.text = "VICTORY";
        bg.color = Color.green;
        UnityAction action = () => ChangeScene("MainMenuScene");
        confirmButton.onClick.AddListener(action);
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ChangeScene(string scenename)
    {
        Time.timeScale = 1f;
        SceneChanger.LoadScene(scenename);
    }
}
