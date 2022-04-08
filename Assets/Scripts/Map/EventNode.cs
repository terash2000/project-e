using UnityEngine;

public class EventNode : Node
{
    private DialogNode _randomEvent;
    public DialogNode RandomEvent
    {
        get { return _randomEvent; }
        set { _randomEvent = value; }
    }

    protected override void ChangeScene()
    {
        DialogManager.NextRoot.Push(_randomEvent);
        SceneChanger.LoadScene("StoryScene");
    }
}
