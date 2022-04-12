using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryMenu : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup _eventContainer;
    [SerializeField] private GameObject _storyEventPrefab;
    [SerializeField] private List<StoryEvent> _eventList;

    private static List<string> _completedStory;

    public static List<string> CompletedStory
    {
        get { return _completedStory; }
        set { _completedStory = value; }
    }

    public void Start()
    {
        List<StoryEvent> uncompleted = _eventList.FindAll(storyEvent =>
            {
                return !_completedStory.Contains(storyEvent.name) && !IsLock(storyEvent);
            });

        List<StoryEvent> completed = _eventList.FindAll(storyEvent => _completedStory.Contains(storyEvent.name));

        foreach (StoryEvent storyEvent in uncompleted)
        {
            GameObject storyButton = Instantiate(_storyEventPrefab, _eventContainer.transform);
            storyButton.GetComponent<StoryButtonHandle>().Init(storyEvent);
        }

        foreach (StoryEvent storyEvent in completed)
        {
            GameObject storyButton = Instantiate(_storyEventPrefab, _eventContainer.transform);
            storyButton.GetComponent<StoryButtonHandle>().Init(storyEvent);
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private bool IsLock(StoryEvent storyEvent)
    {
        return storyEvent.PreviousEvents != null &&
                storyEvent.PreviousEvents.FindAll(storyEvent => !_completedStory.Contains(storyEvent.name)).Count != 0;

    }
}
