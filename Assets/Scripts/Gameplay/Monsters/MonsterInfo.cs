using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MonsterInfo : ScriptableObject
{
    public string MonsterName;
    public int MaxHealth;
    public int InitialBlock;
    public List<MonsterPattern> Patterns;
    public RuntimeAnimatorController AnimatorController;
    public Color SpriteColor = Color.white;
    public float SpriteScale = 1;

    [Serializable]
    public class StatusEffectDictionary : SerializableDictionary<Status.Type, int> { }

    [Serializable]
    public class MonsterPattern
    {
        public MonsterPatternType Pattern;
        public int Damage;
        public int AttackRange;
        public int MoveRange;
        public int BlockGain;
        public StatusEffectDictionary AttackStatusEffect;
    }
}
