using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Wave : ScriptableObject
{
    public List<MonsterSpawner> monsters;

    [Serializable]
    public class MonsterSpawner
    {
        public MonsterInfo monster;
        public Vector2Int tile;
        public int currentMove;
    }
}
