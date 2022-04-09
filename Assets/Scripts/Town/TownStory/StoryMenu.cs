using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryMenu : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup _eventContainer;
    [SerializeField] private GameObject _storyEventPrefab;
    [SerializeField] private List<StoryEvent> _eventList;

    public void Start()
    {
        foreach (StoryEvent storyEvent in _eventList)
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
}
