using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class MonsterInfo : ScriptableObject
{
    public string monsterName;
    public int maxHealth;
    public List<MonsterPattern> patterns;
    public RuntimeAnimatorController animatorController;
    public Color spriteColor = Color.white;
    public float spriteScale = 1;

    [Serializable]
    public class StatusEffectDictionary : SerializableDictionary<Status, int> { }

    [Serializable]
    public class MonsterPattern
    {
        public MonsterPatternType pattern;
        public int damage;
        public int attackRange;
        public int moveRange;
        public StatusEffectDictionary attackStatusEffect;
    }
}
