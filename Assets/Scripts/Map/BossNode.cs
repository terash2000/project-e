using UnityEngine;

public class BossNode : Node
{
    [SerializeField] private DialogNode _bossEvent;

    protected override void ChangeScene()
    {
        DialogManager.NextRoot.Push(_bossEvent);
        SceneChanger.Instance.LoadScene("StoryScene");
    }
}
