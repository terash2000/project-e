using UnityEngine;

public class DamageQueueData
{
    public int damage;
    public int block;
    public Color color;

    public DamageQueueData(int damageParam, int blockParam, Color colorParam)
    {
        damage = damageParam;
        block = blockParam;
        color = colorParam;
    }
}