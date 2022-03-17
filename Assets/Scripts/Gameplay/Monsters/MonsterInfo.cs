using UnityEngine;

[CreateAssetMenu]
public class MonsterInfo : ScriptableObject
{
    public string monsterName;
    public MonsterMovement movement;
    public RuntimeAnimatorController animatorController;
}
