using UnityEngine;

public class TownNode : Node
{
    public const int HEAL = 15;
    protected override void ChangeScene()
    {
        PlayerData.Health += HEAL;
        SceneChanger.Instance.LoadScene("TownScene");
    }
}
