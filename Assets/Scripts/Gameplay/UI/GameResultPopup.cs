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
        text = gameObject.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        bg = gameObject.transform.Find("Panel").gameObject.GetComponent<Image>();
        confirmButton = gameObject.transform.Find("Confirm").gameObject.GetComponent<Button>();
        gameObject.SetActive(false);
    }
    public void onLose()
    {
        text.text = "DEFEAT";
        bg.color = Color.red;
        UnityAction action = () => Resources.FindObjectsOfTypeAll<OptionMenu>()[0].Abandon(true);
        confirmButton.onClick.AddListener(action);
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void onWin()
    {
        text.text = "VICTORY";
        bg.color = Color.green;
        gameObject.SetActive(true);
        UnityAction action = () => Resources.FindObjectsOfTypeAll<OptionMenu>()[0].ReturnMainMenu();
        confirmButton.onClick.AddListener(action);
        Time.timeScale = 0f;
    }
}
