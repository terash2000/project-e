using TMPro;
using UnityEngine;

public class StoryButtonHandle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private DialogNode _eventNode;

    public void Init(StoryEvent storyEvent)
    {
        _eventNode = storyEvent.DialogNode;
        _text.text = storyEvent.Text;
    }

    public void ChangeScene()
    {
        DialogManager.NextRoot.Push(_eventNode);
        SceneChanger.Instance.LoadScene("StoryScene");
        SceneChanger.Instance.NextSceneStack.Push("TownScene");
    }
}
