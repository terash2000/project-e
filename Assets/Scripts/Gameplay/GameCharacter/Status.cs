[System.Serializable]
public class Status
{
    public const float ACID_TO_BLOCK_MULTIPLIER = 2f;
    public const float STRONG_DAMAGE_MULTIPLIER = 2f;
    public const float WEAK_DAMAGE_MULTIPLIER = 0.5f;

    public enum Type
    {
        None,
        Acid,
        Burn,
        Stun,
        Strong,
        Weak,
    }

    public Status(Type type, int value)
    {
        this.type = type;
        this.value = value;
    }

    public Type type;
    public int value;
}
