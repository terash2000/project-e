using TMPro;
using UnityEngine;

public class StoryButtonHandle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private GameObject _mask;

    private DialogNode _eventNode;

    public void Init(StoryEvent storyEvent)
    {
        _eventNode = storyEvent.DialogNode;
        _text.text = storyEvent.Text;

        if (!StoryMenu.CompletedStory.Contains(storyEvent.name))
        {
            _mask.SetActive(false);
        }
        else
        {
            _text.text += " (completed)";
        }
    }

    public void ChangeScene()
    {
        DialogManager.NextRoot.Push(_eventNode);
        SceneChanger.Instance.LoadScene("StoryScene");
        SceneChanger.Instance.NextSceneStack.Push("TownScene");
    }
}
