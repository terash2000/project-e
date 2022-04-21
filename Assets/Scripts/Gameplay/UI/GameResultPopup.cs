using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GameResultPopup : MonoBehaviour
{
    //[SerializeField] private GameObject _soundController;
    [SerializeField] private AudioClip _winSound;
    [SerializeField] private AudioClip _loseSound;
    private const int MIN_GOLD_REWARD = 30;
    private const int MAX_GOLD_REWARD = 50;

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
        UnityAction action = () => SceneChanger.Instance.LoadScene("MainMenuScene");
        _confirmButton.onClick.AddListener(action);
        gameObject.SetActive(true);
        SoundController.Play(_loseSound);
        Time.timeScale = 0f;
    }

    public void OnWin()
    {
        _text.text = "VICTORY";
        _bg.color = Color.green;
        _confirmButton.onClick.AddListener(GainReward);
        gameObject.SetActive(true);
        SoundController.Play(_winSound);
        Time.timeScale = 0f;
    }

    public void GainReward()
    {
        if (MonsterManager.Wave.Reward)
        {
            GainGold();
            GameObject chooseCardPopup = Instantiate(CardCollection.Instance.ChooseCardPopup, transform.parent);
            chooseCardPopup.GetComponent<ChooseCardPopup>().Init();
        }
        else
        {
            SceneChanger.Instance.NextScene();
        }
    }

    public void GainGold()
    {
        int gold = Random.Range(MIN_GOLD_REWARD, MAX_GOLD_REWARD);
        PlayerData.Gold += gold;
    }
}
