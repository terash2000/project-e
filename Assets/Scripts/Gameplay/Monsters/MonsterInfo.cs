using UnityEngine;

[CreateAssetMenu]
public class MonsterInfo : ScriptableObject
{
    public string monsterName;
    public int maxHealth;
    public MonsterPattern pattern;
    public RuntimeAnimatorController animatorController;
}
