using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipsMenu : MonoBehaviour
{
    [SerializeField] private GameObject _tipUI;
    [SerializeField] private TextMeshProUGUI _tipHeader;
    [SerializeField] private TextMeshProUGUI _tipText;
    [SerializeField] private Image _tipImage;


    public void Open()
    {
        gameObject.SetActive(true);
        SoundController.Pause();
        Time.timeScale = 0f;
        GameManager.GameState = GameState.Pause;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameManager.GameState = GameState.Running;
    }

    public void OpenTip(Tip tip)
    {
        _tipUI.SetActive(true);
        _tipHeader.text = tip.Header;
        _tipText.text = tip.Text;
        _tipImage.sprite = tip.Image;
    }
}
