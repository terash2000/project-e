using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryButtonHandle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _banner;
    [SerializeField] private GameObject _mask;

    private DialogNode _eventNode;
    private Color _green = new Color(0f, 0.7f, 0f);

    public void Init(StoryEvent storyEvent)
    {
        _eventNode = storyEvent.DialogNode;
        _text.text = storyEvent.Text;
        _banner.sprite = storyEvent.Banner;

        if (!StoryMenu.CompletedStory.Contains(storyEvent.name))
        {
            _mask.SetActive(false);
        }
        else
        {
            _text.text += " (completed)";
            _text.color = _green;
        }
    }

    public void ChangeScene()
    {
        DialogManager.NextRoot.Push(_eventNode);
        SceneChanger.Instance.LoadScene("StoryScene");
        SceneChanger.Instance.NextSceneStack.Push("TownScene");
    }
}
