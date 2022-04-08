using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GameResultPopup : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private Image _bg;
    private Button _confirmButton;

    void Start()
    {
        GameObject panel = transform.Find("Panel").gameObject;
        _text = panel.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        _bg = panel.GetComponent<Image>();
        _confirmButton = panel.transform.Find("Confirm").gameObject.GetComponent<Button>();
        gameObject.SetActive(false);
    }

    public void OnLose()
    {
        _text.text = "DEFEAT";
        _bg.color = Color.red;
        UnityAction action = () => SceneChanger.LoadScene("MainMenuScene");
        _confirmButton.onClick.AddListener(action);
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnWin()
    {
        _text.text = "VICTORY";
        _bg.color = Color.green;
        UnityAction action = () => ChooseNewCard();
        _confirmButton.onClick.AddListener(ChooseNewCard);
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ChooseNewCard()
    {
        if (MonsterManager.Wave.Reward)
        {
            GameObject chooseCardPopup = Instantiate(CardCollection.Instance.ChooseCardPopup, transform.parent);
            chooseCardPopup.GetComponent<ChooseCardPopup>().Init();
        }
        else
        {
            SceneChanger.NextScene();
        }
    }
}
