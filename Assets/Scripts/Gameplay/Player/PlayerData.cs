using System.Collections.Generic;

// All other class that will use player data should call this static class
public static class PlayerData
{
    private static int _maxHealth;
    private static int _health;
    private static int _maxMana;
    private static int _mana;
    private static int _gold;
    private static string _seedJSON;
    private static List<int> _path;
    private static List<InGameCard> _deck;

    public static int MaxHealth
    {
        get { return _maxHealth; }
        set
        {
            _maxHealth = value;
            if (_maxHealth < 1) _maxHealth = 1;
            if (_health > _maxHealth) _health = _maxHealth;
        }
    }
    public static int Health
    {
        get { return _health; }
        set
        {
            _health = value;
            if (_health < 0) _health = 0;
            else if (_health > _maxHealth) _health = _maxHealth;
        }
    }
    public static int MaxMana
    {
        get { return _maxMana; }
        set
        {
            _maxMana = value;
            if (_maxMana < 0) _maxMana = 0;
        }
    }
    public static int Mana
    {
        get { return _mana; }
        set { _mana = value; }
    }
    public static int Gold
    {
        get { return _gold; }
        set { _gold = value; }
    }
    public static string SeedJSON
    {
        get { return _seedJSON; }
        set { _seedJSON = value; }
    }
    public static List<int> Path
    {
        get { return _path; }
        set { _path = value; }
    }
    public static List<InGameCard> Deck
    {
        get { return _deck; }
        set { _deck = value; }
    }
}
