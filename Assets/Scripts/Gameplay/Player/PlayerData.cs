using System.Collections.Generic;

// All other class that will use player data should call this static class
public static class PlayerData
{
    private static int maxHealth;
    private static int health;
    private static int maxMana;
    private static int mana;
    private static int gold;
    public static string seedJSON;

    public static int MaxHealth
    {
        get { return maxHealth; }
        set
        {
            maxHealth = value;
            if (maxHealth < 0) maxHealth = 0;
        }
    }
    public static int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health < 0) health = 0;
            else if (health > maxHealth) health = maxHealth;
        }
    }
    public static int MaxMana
    {
        get { return maxMana; }
        set
        {
            maxMana = value;
            if (maxMana < 0) maxMana = 0;
        }
    }
    public static int Mana { get; set; }
    public static int Gold { get; set; }
}
