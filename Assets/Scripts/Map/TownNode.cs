using UnityEngine;

public class TownNode : Node
{
    protected override void ChangeScene()
    {
        SceneChanger.Instance.LoadScene("TownScene");
    }
}
