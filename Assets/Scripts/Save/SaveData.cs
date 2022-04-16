using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int maxHealth;
    public int health;
    public int maxMana;
    public int gold;
    public string seedJSON;
    public List<int> path;
    public List<string> deck;
}
