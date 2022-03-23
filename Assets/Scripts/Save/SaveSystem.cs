using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem
{
    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat");

        SaveData data = new SaveData();
        data.maxHealth = PlayerData.maxHealth;
        data.health = PlayerData.health;
        data.maxMana = PlayerData.maxMana;
        data.gold = PlayerData.gold;

        formatter.Serialize(file, data);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath
                    + "/SaveData.dat", FileMode.Open);
            SaveData data = (SaveData)formatter.Deserialize(file);
            file.Close();

            PlayerData.maxHealth = data.maxHealth;
            PlayerData.health = data.health;
            PlayerData.maxMana = data.maxMana;
            PlayerData.gold = data.gold;
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            File.Delete(Application.persistentDataPath + "/SaveData.dat");
        }
    }

    public static void SaveOptionMenu()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Setting.dat");

        UserSetting setting = new UserSetting();
        setting.autoEndTurn = OptionMenu.autoEndTurn;
        setting.showMonstersAttackArea = OptionMenu.showMonstersAttackArea;

        formatter.Serialize(file, setting);
        file.Close();
    }

    public static void LoadOptionMenu()
    {
        if (File.Exists(Application.persistentDataPath + "/Setting.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath
                    + "/Setting.dat", FileMode.Open);
            UserSetting setting = (UserSetting)formatter.Deserialize(file);
            file.Close();

            OptionMenu.autoEndTurn = setting.autoEndTurn;
            OptionMenu.showMonstersAttackArea = setting.showMonstersAttackArea;
        }
        else
        {
            OptionMenu.autoEndTurn = false;
            OptionMenu.showMonstersAttackArea = true;
        }
    }
}
