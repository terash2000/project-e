using UnityEngine;

public class BattleNode : Node
{
    public Wave wave;

    protected override void ChangeScene()
    {
        MonsterManager.wave = wave;
        SceneChanger.LoadScene("CombatScene");
    }
}
