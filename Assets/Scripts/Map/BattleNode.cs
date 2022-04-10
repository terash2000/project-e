using UnityEngine;

public class BattleNode : Node
{
    private Wave _wave;
    public Wave Wave
    {
        get { return _wave; }
        set { _wave = value; }
    }

    protected override void ChangeScene()
    {
        MonsterManager.Wave = _wave;
        SceneChanger.Instance.LoadScene("CombatScene");
    }
}
