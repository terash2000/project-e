using UnityEngine;

public class BattleNode : Node
{
    public Wave wave;

    protected override void ChangeScene()
    {
        MonsterManager.Wave = wave;
        SceneChanger.LoadScene("CombatScene");
    }
}
