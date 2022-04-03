using UnityEngine;

public class EventNode : Node
{
    public DialogNode randomEvent;

    protected override void ChangeScene()
    {
        DialogManager.nextRoot.Push(randomEvent);
        SceneChanger.LoadScene("StoryScene");
    }
}
