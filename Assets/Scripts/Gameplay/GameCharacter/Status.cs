[System.Serializable]
public class Status
{
    public const float ACID_TO_BLOCK_MULTIPLIER = 2f;
    public enum Type
    {
        None,
        Acid,
        Burn,
        Stun
    }

    public Status(Type type, int value)
    {
        this.type = type;
        this.value = value;
    }

    public Type type;
    public int value;
}
