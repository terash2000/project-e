using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class Wave : ScriptableObject
{
    public List<MonsterSpawner> Monsters;
    public bool Reward;

    [Serializable]
    public class MonsterSpawner
    {
        public MonsterInfo Monster;
        public Vector2Int Tile;
        public int CurrentMove;
    }
}
