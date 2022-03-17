using UnityEngine;

[CreateAssetMenu]
public class MonsterInfo : ScriptableObject
{
    public string monsterName;
    public int maxHealth;
    public MonsterMovement movement;
    public RuntimeAnimatorController animatorController;
}
