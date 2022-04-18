[System.Serializable]
public class Status
{
    public const float ACID_TO_BLOCK_MULTIPLIER = 2f;
    public enum Type
    {
        Stun,
        Acid,
        Burn,
    }

    public Type type;
    public int value;
}
