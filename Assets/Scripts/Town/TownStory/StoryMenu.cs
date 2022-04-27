using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StoryMenu : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup _eventContainer;
    [SerializeField] private GameObject _storyEventPrefab;
    [SerializeField] private List<StoryEvent> _eventList;

    private static List<string> _completedStory;
    private static List<string> _paidStory;

    public static List<string> CompletedStory
    {
        get { return _completedStory; }
        set { _completedStory = value; }
    }
    public static List<string> PaidStory
    {
        get { return _paidStory; }
        set { _paidStory = value; }
    }

    public void Start()
    {
        List<StoryEvent> availableStory = _eventList.FindAll(storyEvent => !IsAvailable(storyEvent));

        availableStory = availableStory.OrderBy(storyEvent => _completedStory.Contains(storyEvent.name)).ToList();

        foreach (StoryEvent storyEvent in availableStory)
        {
            GameObject storyButton = Instantiate(_storyEventPrefab, _eventContainer.transform);
            storyButton.GetComponent<StoryButtonHandle>().Init(storyEvent);

            if (storyEvent.UnlockCost > 0 && !StoryMenu.PaidStory.Contains(storyEvent.name))
            {
                storyButton.GetComponent<GoldChecker>().GoldNeeded = storyEvent.UnlockCost;
            }
            else
            {
                Destroy(storyButton.GetComponent<GoldChecker>());
            }
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

    private bool IsAvailable(StoryEvent storyEvent)
    {
        return storyEvent.PreviousEvents != null &&
                storyEvent.PreviousEvents.FindAll(storyEvent => !_completedStory.Contains(storyEvent.name)).Count != 0;

    }
}
