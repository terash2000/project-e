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
        AddStatus,
        CreateCard
    }

    public Type type;
    public int amount;
    public Status.Type statusType;
    public Card card;
}
