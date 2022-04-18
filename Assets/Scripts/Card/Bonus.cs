using System;

[Serializable]
public class Bonus
{
    public enum Type
    {
        Mana,
        Damage
    }

    public Type type;
    public int amount;
}
