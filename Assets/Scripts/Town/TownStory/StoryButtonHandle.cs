using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoryButtonHandle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _banner;
    [SerializeField] private GameObject _mask;
    [SerializeField] private GameObject _confirmationPopup;

    private StoryEvent _storyEvent;
    private Color _originalColor;
    private Color _orange = new Color(1f, 0.4f, 0f);
    private Color _green = new Color(0f, 0.7f, 0f);

    public void Init(StoryEvent storyEvent)
    {
        _storyEvent = storyEvent;
        _text.text = storyEvent.Text;
        _banner.sprite = storyEvent.Banner;
        _originalColor = _text.color;

        if (!StoryMenu.CompletedStory.Contains(storyEvent.name))
        {
            _mask.SetActive(false);

            if (storyEvent.UnlockCost > 0 && !StoryMenu.PaidStory.Contains(storyEvent.name))
            {
                _text.text += $" (Use {storyEvent.UnlockCost} Gold to unlock)";
                _text.color = _orange;
            }
        }
        else
        {

            _text.text += " (completed)";
            _text.color = _green;
        }
    }

    public void Click()
    {
        if (_storyEvent.UnlockCost > 0 && !StoryMenu.PaidStory.Contains(_storyEvent.name))
        {
            PayToUnlockEvent();
        }
        else
        {
            DialogManager.NextRoot.Push(_storyEvent.DialogNode);
            SceneChanger.Instance.LoadScene("StoryScene");
            SceneChanger.Instance.NextSceneStack.Push("TownScene");
        }
    }

    public void PayToUnlockEvent(bool confirm = false)
    {
        if (confirm)
        {
            PlayerData.Gold -= _storyEvent.UnlockCost;
            StoryMenu.PaidStory.Add(_storyEvent.name);
            _text.text = _storyEvent.Text;
            _text.color = _originalColor;
            Destroy(GetComponent<GoldChecker>());

            SaveSystem.Save();
            SaveSystem.SaveUnlockData();
        }
        else
        {
            GameObject newPopup = Instantiate(_confirmationPopup, transform.parent.parent.parent);
            string message = $"Use {_storyEvent.UnlockCost} Gold to unlock this event.";
            UnityAction action = () => PayToUnlockEvent(true);
            newPopup.GetComponent<ConfirmationPopup>().Init(message, action);
        }
    }
}
