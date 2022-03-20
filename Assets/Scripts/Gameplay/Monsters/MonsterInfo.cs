using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MonsterInfo : ScriptableObject
{
    public string monsterName;
    public int maxHealth;
    public List<MonsterPattern> patterns;
    public RuntimeAnimatorController animatorController;

    [Serializable]
    public class MonsterPattern
    {
        public MonsterPatternType pattern;
        public int damage;
    }
}
