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
        data.maxHealth = PlayerData.MaxHealth;
        data.health = PlayerData.Health;
        data.maxMana = PlayerData.MaxMana;
        data.gold = PlayerData.Gold;
        data.seedJSON = PlayerData.seedJSON;
        data.path = PlayerData.path;

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

            PlayerData.MaxHealth = data.maxHealth;
            PlayerData.Health = data.health;
            PlayerData.MaxMana = data.maxMana;
            PlayerData.Gold = data.gold;
            PlayerData.seedJSON = data.seedJSON;
            PlayerData.path = data.path;
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

    public static void SaveUnlockData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/UnlockData.dat");

        UnlockData data = new UnlockData();
        data.unlockDict = CardCollection.unlockDict;

        formatter.Serialize(file, data);
        file.Close();
    }

    public static void LoadUnlockData()
    {
        if (File.Exists(Application.persistentDataPath + "/UnlockData.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath
                    + "/UnlockData.dat", FileMode.Open);
            UnlockData data = (UnlockData)formatter.Deserialize(file);
            file.Close();

            if (data.unlockDict.Count == CardCollection.singleton.allCards.Count)
            {
                CardCollection.unlockDict = data.unlockDict;
            }
            else
            {
                // corrupted data or old verision
                File.Delete(Application.persistentDataPath + "/UnlockData.dat");
                CardCollection.unlockDict = CardCollection.singleton.StarterCards();
            }
        }
        else
        {
            CardCollection.unlockDict = CardCollection.singleton.StarterCards();
        }
    }
}
