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
        UnityAction action = () => SceneChanger.LoadScene("MainMenuScene");
        confirmButton.onClick.AddListener(action);
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void onWin()
    {
        text.text = "VICTORY";
        bg.color = Color.green;
        UnityAction action = () => ChooseNewCard();
        confirmButton.onClick.AddListener(action);
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ChooseNewCard()
    {
        GameObject chooseCardPopup = Instantiate(CardCollection.Instance.chooseCardPopup, transform.parent);
        chooseCardPopup.GetComponent<ChooseCardPopup>().Init();
    }
}
