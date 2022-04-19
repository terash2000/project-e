using System;

[Serializable]
public class Bonus
{
    public enum Type
    {
        Mana,
        Damage,
        Radius,
        CastRange
    }

    public Type type;
    public int amount;
}
