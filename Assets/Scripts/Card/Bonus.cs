using System;

[Serializable]
public class Bonus
{
    public enum Type
    {
        Mana,
        Damage,
        Radius,
        CastRange,
        AddStatus
    }

    public Type type;
    public int amount;
    public Status.Type statusType;
}
